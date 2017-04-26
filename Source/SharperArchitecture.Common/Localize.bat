SET ProjectDir=%~dp0
SET clientLocFolder=Properties\Client
SET serverToClientRegex=s/{\(.\)\(\w*\)}/{{\l\1\2}}/g
SET clientToServerRegex=s/{{\(.\)\(\w*\)}}/\{\u\1\2\}/g
SET xgettextPath=..\packages\GNU.Tools.1.18\xgettext.exe
SET msgcatPath=..\packages\GNU.Tools.1.18\msgcat.exe
SET sedPath=..\packages\Sed.4.2.1\sed.exe

@Rem create a POT file for all cs files (Server side)
cd %ProjectDir%
dir ..\*.cs /S /B > Strings.filelist
%xgettextPath% -kI18N.Register -kI18N.Translate -kTranslatePlural -LC# -fStrings.filelist --no-location --from-code=UTF-8 -o ".\server.pot"
del /Q /F Strings.filelist

@Rem create a POT file for all html/js files (Client side). 
@Rem call grunt nggettext_extract
@Rem replace client format to server format (ie, %test% will become {Test})
@Rem %sedPath% -ie %clientToServerRegex% client.pot

@Rem remove temp file that sed lefts
del /Q /F sed*
del /Q /F *.pote

@Rem merge client and server pot files
@Rem msgcatPath% *.pot --use-first > Properties\Messages.pot
@Rem del /Q /F *.pot

@Rem create client localization folder
@Rem if not exist %ProjectDir%%clientLocFolder% mkdir %ProjectDir%%clientLocFolder%

@Rem replace server format to client format (ie, %test% will become {Test})
@Rem %sedPath% -e %serverToClientRegex% Properties\Messages.pot > %clientLocFolder%\Messages.pot

@REM for statement body must not have parenthesis char so goto is used :S 
@Rem for %%i in (.\Properties\*.po) DO CALL :loopbody %%i
@Rem GOTO :nggettext_compile

@Rem :loopbody
@Rem for %%F in (%1) do set fileName=%%~nxF
@Rem %sedPath% -e %serverToClientRegex% %1 > %clientLocFolder%\%fileName%
@Rem GOTO :EOF

@Rem :nggettext_compile
@Rem create a js file with all translations
@Rem call grunt nggettext_compile

@Rem remove client localization folder once the translations was created
@Rem rmdir %ProjectDir%%clientLocFolder% /s /q





pause