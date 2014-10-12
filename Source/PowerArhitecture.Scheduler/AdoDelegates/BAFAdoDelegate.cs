using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BAF.Scheduler.Filters;
using BAF.Scheduler.Specifications;
using HibernatingRhinos.Profiler.Appender;
using Quartz.Impl.Matchers;
using Quartz.Impl.Triggers;
using Quartz.Util;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.AdoJobStore;
using Quartz.Spi;

namespace BAF.Scheduler.AdoDelegates
{
    public class BAFAdoDelegate : StdAdoDelegate, IBAFDriverDelegate
    {
        static BAFAdoDelegate()
        {
            SetField("SqlInsertTrigger", SqlInsertTrigger
                .Replace(string.Format("{0})", ColumnPriority), string.Format("{0}, {1}, {2})",
                    ColumnPriority, ColumnDateCreated, ColumnCreatedBy))
                .Replace("@triggerPriority)", "@triggerPriority, @currentDateTime, @createdBy)"));

            SetField("SqlInsertJobDetail", SqlInsertJobDetail
                .Replace(string.Format("{0})", ColumnJobDataMap), string.Format("{0}, {1}, {2})", ColumnJobDataMap, ColumnCreatedBy, ColumnDateCreated))
                .Replace("@jobDataMap)", "@jobDataMap, @createdBy, @currentDateTime)"));

            SetField("SqlSelectJobDetail", SqlSelectJobDetail
                .Replace(" FROM", string.Format(",{0},{1} FROM", ColumnCreatedBy, ColumnDateCreated)));

            SetField("SqlSelectTrigger", SqlSelectTrigger
                .Replace(" FROM", string.Format(",{0},{1} FROM", ColumnDateCreated, ColumnCreatedBy)));

            SetField("SqlSelectJobForTrigger", SqlSelectJobForTrigger
                .Replace(" FROM", string.Format(",J.{0},J.{1} FROM", ColumnCreatedBy, ColumnDateCreated)));
        }

        #region Queries

        public static readonly string SqlSelectTriggerKeys =
            string.Format(CultureInfo.InvariantCulture, "SELECT {0}, {1} FROM {2}{3} WHERE {4} = {5}",
                ColumnTriggerName, ColumnTriggerGroup, TablePrefixSubst, TableTriggers, ColumnSchedulerName, SchedulerNameSubst);

        public static readonly string SqlSelectJobKeys =
            string.Format(CultureInfo.InvariantCulture, "SELECT {0}, {1} FROM {2}{3} WHERE {4} = {5}",
                ColumnJobName, ColumnJobGroup, TablePrefixSubst, TableJobDetails, ColumnSchedulerName, SchedulerNameSubst);

        #endregion

        #region Columns

        public const string ColumnCreatedBy = "CREATED_BY";

        public const string ColumnDateCreated = "DATE_CREATED";

        #endregion

        public override IJobDetail SelectJobForTrigger(ConnectionAndTransactionHolder conn, TriggerKey triggerKey, ITypeLoadHelper loadHelper, bool loadJobType)
        {
            using (var cmd = PrepareCommand(conn, ReplaceTablePrefix(SqlSelectJobForTrigger)))
            {
                AddCommandParameter(cmd, "triggerName", triggerKey.Name);
                AddCommandParameter(cmd, "triggerGroup", triggerKey.Group);
                using (var rs = cmd.ExecuteReader())
                {
                    if (!rs.Read()) return null;
                    var job = new BAFJobDetailImpl
                    {
                        Name = rs.GetString(ColumnJobName),
                        Group = rs.GetString(ColumnJobGroup),
                        Durable = GetBooleanFromDbValue(rs[ColumnIsDurable]),
                        DateCreated = GetDateTimeFromDbValue(rs[ColumnDateCreated]).GetValueOrDefault(),
                        CreatedBy = rs.GetString(ColumnCreatedBy)
                    };
                    if (loadJobType)
                    {
                        job.JobType = loadHelper.LoadType(rs.GetString(ColumnJobClass));
                    }
                    job.RequestsRecovery = GetBooleanFromDbValue(rs[ColumnRequestsRecovery]);

                    return job;
                }
            }
        }

        public override IJobDetail SelectJobDetail(ConnectionAndTransactionHolder conn, JobKey jobKey, ITypeLoadHelper loadHelper)
        {
            using (var cmd = PrepareCommand(conn, ReplaceTablePrefix(SqlSelectJobDetail)))
            {
                AddCommandParameter(cmd, "jobName", jobKey.Name);
                AddCommandParameter(cmd, "jobGroup", jobKey.Group);
                using (var rs = cmd.ExecuteReader())
                {
                    if (!rs.Read()) return null;
                    var job = new BAFJobDetailImpl
                    {
                        Name = rs.GetString(ColumnJobName),
                        Group = rs.GetString(ColumnJobGroup),
                        Description = rs.GetString(ColumnDescription),
                        JobType = loadHelper.LoadType(rs.GetString(ColumnJobClass)),
                        Durable = GetBooleanFromDbValue(rs[ColumnIsDurable]),
                        RequestsRecovery = GetBooleanFromDbValue(rs[ColumnRequestsRecovery]),
                        DateCreated = GetDateTimeFromDbValue(rs[ColumnDateCreated]).GetValueOrDefault(),
                        CreatedBy = rs.GetString(ColumnCreatedBy)
                    };

                    var map = CanUseProperties ? GetMapFromProperties(rs, 6) : GetObjectFromBlob<IDictionary>(rs, 6);

                    if (map != null)
                    {
                        job.JobDataMap = map as JobDataMap ?? new JobDataMap(map);
                    }

                    return job;
                }
            }
        }

        public override IOperableTrigger SelectTrigger(ConnectionAndTransactionHolder conn, TriggerKey triggerKey)
        {
            IOperableTrigger trigger = null;

            using (var cmd = PrepareCommand(conn, ReplaceTablePrefix(SqlSelectTrigger)))
            {
                AddCommandParameter(cmd, "triggerName", triggerKey.Name);
                AddCommandParameter(cmd, "triggerGroup", triggerKey.Group);

                using (var rs = cmd.ExecuteReader())
                {
                    if (!rs.Read()) return null;
                    string jobName = rs.GetString(ColumnJobName);
                    string jobGroup = rs.GetString(ColumnJobGroup);
                    string description = rs.GetString(ColumnDescription);
                    string triggerType = rs.GetString(ColumnTriggerType);
                    string calendarName = rs.GetString(ColumnCalendarName);
                    int misFireInstr = rs.GetInt32(ColumnMifireInstruction);
                    int priority = rs.GetInt32(ColumnPriority);
                    var dateCreated = GetDateTimeFromDbValue(rs[ColumnDateCreated]).GetValueOrDefault();
                    var createdBy = rs.GetString(ColumnCreatedBy);

                    IDictionary map = CanUseProperties ? GetMapFromProperties(rs, 11) : GetObjectFromBlob<IDictionary>(rs, 11);

                    DateTimeOffset? nextFireTimeUtc = GetDateTimeFromDbValue(rs[ColumnNextFireTime]);
                    DateTimeOffset? previousFireTimeUtc = GetDateTimeFromDbValue(rs[ColumnPreviousFireTime]);
                    DateTimeOffset startTimeUtc = GetDateTimeFromDbValue(rs[ColumnStartTime]) ?? DateTimeOffset.MinValue;
                    DateTimeOffset? endTimeUtc = GetDateTimeFromDbValue(rs[ColumnEndTime]);

                    // done reading
                    rs.Close();

                    if (triggerType.Equals(TriggerTypeBlob))
                    {
                        using (IDbCommand cmd2 = PrepareCommand(conn, ReplaceTablePrefix(SqlSelectBlobTrigger)))
                        {
                            AddCommandParameter(cmd2, "triggerName", triggerKey.Name);
                            AddCommandParameter(cmd2, "triggerGroup", triggerKey.Group);
                            using (IDataReader rs2 = cmd2.ExecuteReader())
                            {
                                if (rs2.Read())
                                {
                                    trigger = GetObjectFromBlob<IOperableTrigger>(rs2, 0);
                                }
                            }
                        }
                    }
                    else
                    {
                        ITriggerPersistenceDelegate tDel = FindTriggerPersistenceDelegate(triggerType);

                        if (tDel == null)
                        {
                            throw new JobPersistenceException("No TriggerPersistenceDelegate for trigger discriminator type: " + triggerType);
                        }

                        TriggerPropertyBundle triggerProps;
                        try
                        {
                            triggerProps = tDel.LoadExtendedTriggerProperties(conn, triggerKey);
                        }
                        catch (InvalidOperationException)
                        {
                            if (IsTriggerStillPresent(cmd))
                            {
                                throw;
                            }
                            else
                            {
                                // QTZ-386 Trigger has been deleted
                                return null;
                            }
                        }

                        TriggerBuilder tb = TriggerBuilder.Create()
                            .WithDescription(description)
                            .WithPriority(priority)
                            .StartAt(startTimeUtc)
                            .EndAt(endTimeUtc)
                            .WithIdentity(triggerKey)
                            .ModifiedByCalendar(calendarName)
                            .WithSchedule(triggerProps.ScheduleBuilder)
                            .ForJob(new JobKey(jobName, jobGroup));

                        if (map != null)
                        {
                            tb.UsingJobData(map as JobDataMap ?? new JobDataMap(map));
                        }

                        trigger = (IOperableTrigger)tb.Build();

                        trigger.MisfireInstruction = misFireInstr;
                        trigger.SetNextFireTimeUtc(nextFireTimeUtc);
                        trigger.SetPreviousFireTimeUtc(previousFireTimeUtc);

                        SetTriggerStateProperties(trigger, triggerProps);

                        //Added
                        if (trigger is SimpleTriggerImpl)
                            trigger = new SimpleTriggerWrapper((SimpleTriggerImpl)trigger)
                            {
                                DateCreated = dateCreated,
                                CreatedBy = createdBy
                            };
                        else if (trigger is CronTriggerImpl)
                            trigger = new CronTriggerWrapper((CronTriggerImpl)trigger)
                            {
                                DateCreated = dateCreated,
                                CreatedBy = createdBy
                            };
                        else if (trigger is CalendarIntervalTriggerImpl)
                            trigger = new CalendarIntervalTriggerWrapper((CalendarIntervalTriggerImpl)trigger)
                            {
                                DateCreated = dateCreated,
                                CreatedBy = createdBy
                            };
                        else if (trigger is DailyTimeIntervalTriggerImpl)
                            trigger = new DailyTimeIntervalTriggerWrapper((DailyTimeIntervalTriggerImpl)trigger)
                            {
                                DateCreated = dateCreated,
                                CreatedBy = createdBy
                            };
                    }
                }
            }

            return trigger;
        }

        public virtual Quartz.Collection.ISet<JobKey> SelectJobs(ConnectionAndTransactionHolder conn, JobFilter filter)
        {
            using (var cmd = PrepareCommand(conn))
            {
                ApplySelectFilter(cmd, filter);
                using (var rs = cmd.ExecuteReader())
                {
                    var list = new Quartz.Collection.HashSet<JobKey>();
                    while (rs.Read())
                    {
                        list.Add(new JobKey(rs.GetString(0), rs.GetString(1)));
                    }

                    return list;
                }
            }
        }

        public virtual Quartz.Collection.ISet<TriggerKey> SelectTriggers(ConnectionAndTransactionHolder conn, TriggerFilter filter)
        {
            using (var cmd = PrepareCommand(conn))
            {
                ApplySelectFilter(cmd, filter);
                using (var rs = cmd.ExecuteReader())
                {
                    var keys = new Quartz.Collection.HashSet<TriggerKey>();
                    while (rs.Read())
                    {
                        keys.Add(new TriggerKey(rs.GetString(0), rs.GetString(1)));
                    }

                    return keys;
                }
            }
        }

        #region PrepareCommand

        public IDbCommand PrepareCommand(ConnectionAndTransactionHolder cth)
        {
            return PrepareCommand(cth, null);
        }

        public override IDbCommand PrepareCommand(ConnectionAndTransactionHolder cth, string commandText)
        {
            var cmd = base.PrepareCommand(cth, commandText);

            if (!cmd.Parameters.Contains("createdBy") && commandText != null && commandText.StartsWith("insert", StringComparison.CurrentCultureIgnoreCase))
                AddCommandParameter(cmd, "createdBy", "CurrentUser"); //Used for inserts*/
            AddCommandParameter(cmd, "currentDateTime", GetDbDateTimeValue(DateTime.UtcNow));
            if(!string.IsNullOrEmpty(cmd.CommandText))
                DebugSql(cmd);
            return cmd;
        }

        #endregion

        #region Private functions

        private void ApplySelectFilter(IDbCommand cmd, TriggerFilter filter)
        {
            ApplyFilter(cmd, filter, SqlSelectTriggerKeys);
        }

        //For select and update queries
        private void ApplyFilter(IDbCommand cmd, TriggerFilter filter, string query)
        {
            //All select and update queries have where statement
            var whereIndex = query.IndexOf("where", StringComparison.InvariantCultureIgnoreCase);
            query = ReplaceTablePrefix(query);

            if (query.IndexOf(ColumnTriggerGroup, whereIndex, StringComparison.InvariantCulture) == -1)
            {
                if (IsMatcherEquals(filter.GroupMatcher))
                {
                    var group = ToSqlEqualsClause(filter.GroupMatcher);
                    query = query + string.Format(" AND {0} = @triggerGroup", ColumnTriggerGroup);
                    AddCommandParameter(cmd, "triggerGroup", group);
                }
                else
                {
                    var group = ToSqlLikeClause(filter.GroupMatcher);
                    query = query + string.Format(" AND {0} LIKE @triggerGroup", ColumnTriggerGroup);
                    AddCommandParameter(cmd, "triggerGroup", group);
                }
            }
            if (!string.IsNullOrEmpty(filter.CreatedBy) && query.IndexOf(ColumnCreatedBy, whereIndex, StringComparison.InvariantCulture) == -1)
            {
                query = query + string.Format(" AND {0} = @createdBy", ColumnCreatedBy);
                AddCommandParameter(cmd, "createdBy", filter.CreatedBy);
            }

            cmd.CommandText = query;
            DebugSql(cmd);
        }

        private void ApplySelectFilter(IDbCommand cmd, JobFilter filter)
        {
            var query = SqlSelectJobKeys;
            //All select queries have where statement
            //var existWhere = selectQuery.IndexOf("where", StringComparison.InvariantCultureIgnoreCase) > -1;
            query = ReplaceTablePrefix(query);

            if (IsMatcherEquals(filter.GroupMatcher))
            {
                var group = ToSqlEqualsClause(filter.GroupMatcher);
                query = query + string.Format(" AND {0} = @jobGroup", ColumnJobGroup);
                AddCommandParameter(cmd, "jobGroup", group);
            }
            else
            {
                var group = ToSqlLikeClause(filter.GroupMatcher);
                query = query + string.Format(" AND {0} LIKE @jobGroup", ColumnJobGroup);
                AddCommandParameter(cmd, "jobGroup", group);
            }

            if (!string.IsNullOrEmpty(filter.CreatedBy))
            {
                query = query + string.Format(" AND {0} = @createdBy", ColumnCreatedBy);
                AddCommandParameter(cmd, "createdBy", filter.CreatedBy);
            }

            cmd.CommandText = query;
            DebugSql(cmd);
        }

        private static bool IsTriggerStillPresent(IDbCommand command)
        {
            using (var rs = command.ExecuteReader())
            {
                return rs.Read();
            }
        }

        private static void SetTriggerStateProperties(IOperableTrigger trigger, TriggerPropertyBundle props)
        {
            if (props.StatePropertyNames == null)
            {
                return;
            }

            ObjectUtils.SetObjectProperties(trigger, props.StatePropertyNames, props.StatePropertyValues);
        }

        /// <summary> build Map from java.util.Properties encoding.</summary>
        private IDictionary GetMapFromProperties(IDataReader rs, int idx)
        {
            var properties = GetJobDataFromBlob<NameValueCollection>(rs, idx);
            if (properties == null)
            {
                return null;
            }
            var map = ConvertFromProperty(properties);
            return map;
        }

        private static void SetField(string name, object value)
        {
            var field = typeof(StdAdoConstants).GetField(name);
            if(field == null)
                throw new NullReferenceException("field");
            field.SetValue(null, value);
        }

        private void DebugSql(IDbCommand command)
        {
            var query = command.CommandText;
            foreach (IDbDataParameter param in command.Parameters)
            {
                query = query.Replace(string.Format("{0}", param.ParameterName), string.Format("'{0}'", param.Value));
            }
            ProfilerIntegration.PublishProfilerEvent(Guid.NewGuid().ToString(), "NHibernate.SQL", query);
        }

        #endregion
    }
}
