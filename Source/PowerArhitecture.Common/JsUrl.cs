using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BAF.Common
{
    /// <summary>
    /// JSURL is an alternative to JSON + URL encoding (or JSON + base64 encoding). It makes it handy to pass complex values via URL query parameters.
    /// https://github.com/Sage/jsurl/blob/master/lib/jsurl.js
    /// </summary>
    public class JsUrl
    {
        private int _i;
        private int _len;
        private string _s;

        private readonly Dictionary<string, object> _reserved = new Dictionary<string, object>()
        {
            {"true", true},
            {"false", false},
            {"null", null}
        };

        public static string Parse(string value)
        {
            return new JsUrl().ParseInternal(value);
        }

        private string ParseInternal(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            _i = 0;
            _len = value.Length;
            _s = value;
            var obj = ParseOne();
            return JsonConvert.SerializeObject(obj);
        }

        private object ParseOne()
        {
            dynamic result;
            int beg;
            char ch;
            Eat('~');
            switch (ch = _s[_i])
            {
                case '(':
                    _i++;
                    if (_s[_i] == '~')
                    {
                        result = new List<object>();
                        if (_s[_i + 1] == ')') _i++;
                        else
                        {
                            do
                            {
                                result.Add(ParseOne());
                            } while (_s[_i] == '~');
                        }
                    }
                    else
                    {
                        result = new Dictionary<string, object>();
                        if (_s[_i] != ')')
                        {
                            do
                            {
                                var key = Decode();
                                result[key] = ParseOne();
                            } while (_s[_i] == '~' && (++_i != 0));
                        }
                    }
                    Eat(')');
                    break;
                case '\'':
                    _i++;
                    result = Decode();
                    break;
                default:
                    beg = _i++;
                    while (_i < _len && Regex.IsMatch(_s[_i].ToString(CultureInfo.InvariantCulture), @"[^)~]"))
                        _i++;
                    var sub = JsSubString(_s, beg, _i);
                    if (Regex.IsMatch(ch.ToString(CultureInfo.InvariantCulture), @"[\d\-]"))
                        result = Convert.ToDouble(sub);
                    else
                    {
                        if(!_reserved.ContainsKey(sub))
                            throw new FormatException("Bad value keyword: " + sub);
                        result = _reserved[sub];
                    }
                    break;
            }
            return result;
        }
        

        private string Decode()
        {
            var beg = _i;
            var r = "";
            char ch;
            while (_i < _len && (ch = _s[_i]) != '~' && ch != ')')
            {
                switch (ch)
                {
                        
                    case '*':
                        if (beg < _i) r += JsSubString(_s, beg, _i);
                        if (_s[_i + 1] == '*')
                        {
                            r += Convert.ToChar(Convert.ToInt32(JsSubString(_s, _i + 2, _i + 6), 16));
                            beg = (_i += 6);
                        }
                        else
                        {
                            r += Convert.ToChar(Convert.ToInt32(JsSubString(_s, _i + 1, _i + 3), 16));
                            beg = (_i += 3);
                        }
                        break;
                    case '!':
                        if (beg < _i) r += JsSubString(_s, beg, _i);
                        r += '$';
                        beg = ++_i;
                        break;
                    default:
                        _i++;
                        break;
                }
            }
            return r + JsSubString(_s, beg, _i);
        }

        private void Eat(char expected)
        {
            if(_s[_i] != expected)
                throw new FormatException("Bad JSURL syntax: expected " + expected + ", got " + _s[_i]);
            _i++;
        }

        private static string JsSubString(string value, int start, int? end = null)
        {
            if (!end.HasValue) return value.Substring(start);

            if (start > end.Value) //If "start" is greater than "end", it will swap the two arguments
            {
                var tmp = start;
                start = end.Value;
                end = tmp;
            }
            if (start < 0)
                start = 0;

            return value.Substring(start, end.Value - start);
        }

    }
}
