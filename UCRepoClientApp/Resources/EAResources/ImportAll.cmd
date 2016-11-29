cls
ECHO OFF
set theRepo=.\EAResources\UCRepo.eap
set theInPath=.\FilledInTemplates
FOR /F  "delims=" %%i in ('dir "%theInPath%\*.docx" /b') do (
@echo Processing =====]  %%i
ucrepoclientapp.cmd importword "repo=%theRepo%" "inpath=%theInPath%" "infile=%%i"
)
ECHO ON