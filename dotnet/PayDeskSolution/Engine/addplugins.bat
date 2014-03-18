@echo off

set PROJECT=..\
set APP_HOME=bin\Debug
set PLUGINS_HOME=%APP_HOME%\Plugins


if not exist %PLUGINS_HOME%\DATECS_EXELLIO (
    echo "mkdir %PLUGINS_HOME%\DATECS_EXELLIO"
    mkdir %PLUGINS_HOME%\DATECS_EXELLIO
)
copy %PROJECT%\Plugin_DATECS_EXELLIO\bin\Debug\DATECS_EXELLIO.dll %PLUGINS_HOME%\DATECS_EXELLIO\


if not exist %PLUGINS_HOME%\DATECS_FP3530T (
    echo "mkdir %PLUGINS_HOME%\DATECS_FP3530T"
    mkdir %PLUGINS_HOME%\DATECS_FP3530T
)
copy %PROJECT%\Plugin_DATECS_FP3530T\bin\Debug\DATECS_FP3530T.dll %PLUGINS_HOME%\DATECS_FP3530T\


if not exist %PLUGINS_HOME%\IKC-OP2 (
    echo "mkdir %PLUGINS_HOME%\IKC-OP2"
    mkdir %PLUGINS_HOME%\IKC-OP2
)
copy %PROJECT%\Plugin_IKC-OP2\bin\Debug\IKC-OP2.dll %PLUGINS_HOME%\IKC-OP2\


if not exist %PLUGINS_HOME%\MINI-FP6 (
    echo "mkdir %PLUGINS_HOME%\MINI-FP6"
    mkdir %PLUGINS_HOME%\MINI-FP6
)
copy %PROJECT%\Plugin_MINI-FP6\bin\Debug\MINI-FP6.dll %PLUGINS_HOME%\MINI-FP6\