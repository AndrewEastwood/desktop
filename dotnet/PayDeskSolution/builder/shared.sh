#!/bin/bash

# DATAROOT="$( cd $( dirname "$0" ) && pwd )"
COLORSENABLED=true
PROJECT=./../
DEBUG=./../Engine/bin/Debug
APP_HOME=./bin

HR="|---------------------------------------------------------------------"

function chat()
{
    if [ $COLORSENABLED = "true" ]
    then
        echo " "
        echo -e "\033[38;5;148m$1\033[39m"
        echo " "
    else
        echo " "
        echo $1
        echo ""
    fi
}

function start()
{
    echo $HR
    if [ $COLORSENABLED = "true" ]
    then
        echo -e "| Starting: \033[38;5;148m$1\033[39m"
    else
        echo -e "| Starting: $1"
    fi
}

function end()
{
    echo " "
    if [ $COLORSENABLED = "true" ]
    then
        echo -e "| Finished: \033[38;5;148m$1\033[39m"
    else
        echo -e "| Finished: $1"
    fi
    echo $HR
}

function cleanup() {
    chat "cleanup"
    rm -rf $APP_HOME/*
}

function getVersion() {
    echo "Build Version: `date +%y.%m%d.%H%M`"
}

function getVersionNumber() {
    echo "`date +%y.%m%d.%H%M`"
}

function getVersionNumberFile() {
    echo "`date +%y-%m%d-%H%M`"
}

function buildTest() {

    chat "Test Build Started"

    APP_HOME=./test

    mkdir $APP_HOME
    
    _updateLibs
    _updateStatic
    _updatePlugins
    _copyTools
}

function buildPatch() {

    chat "Patch Build Started"

    _updateLibs
    _updatePlugins
    _createAppPatch "patch"
}

function buildProduction() {

    chat "Production Build Started"

    mkdir $APP_HOME

    _updateLibs
    _updateStatic
    _updatePlugins
    _copyTools
    _setBuildVersion
    _cleanGeneratedData
    _createAppPatch "prod"
}

function _updateLibs() {
    #
    # copy libraries
    #
    start "copy libraries"
    cp -v $DEBUG/PayDesk.exe $APP_HOME/
    cp -v $DEBUG/driver.dll $APP_HOME/
    cp -v $DEBUG/components.dll $APP_HOME/
    cp -v $DEBUG/HtmlAgilityPack.dll $APP_HOME/
    cp -v $DEBUG/Ionic.Zip.dll $APP_HOME/
    cp -v $DEBUG/Mono.Security.dll $APP_HOME/
    cp -v $DEBUG/Npgsql.dll $APP_HOME/
    end "copy libraries"
}

function _updateStatic() {
    #
    # copy source
    #
    start "copy source"
    for f in ./src/*
    do
        if [ -d "$f" ]
        then
            cp -rf "$f" $APP_HOME/
        elif [ -f "$f" ]
        then
            cp -rfv "$f" $APP_HOME/
        fi
    done
    # cp -rv ./src/* $APP_HOME/
    end "copy source"
}

function _cleanGeneratedData() {
    #
    # clean generated data
    #
    start "clean generated data"
    rm -rfv $APP_HOME/articles/*
    rm -rfv $APP_HOME/bills/*
    rm -rfv $APP_HOME/cheques/*
    rm -rfv $APP_HOME/reports/*
    end "clean generated data"
}

function _updatePlugins() {
    #
    # copy (with overwrite) plugins
    #
    PLUGINS_HOME=$APP_HOME/Plugins
    start "copy (with overwrite) plugins"
    mkdir -p $PLUGINS_HOME/DATECS_EXELLIO
    cp -fv $PROJECT/Plugin_DATECS_EXELLIO/bin/Debug/DATECS_EXELLIO.dll $PLUGINS_HOME/DATECS_EXELLIO/
    mkdir -p $PLUGINS_HOME/DATECS_FP3530T
    cp -fv $PROJECT/Plugin_DATECS_FP3530T/bin/Debug/DATECS_FP3530T.dll $PLUGINS_HOME/DATECS_FP3530T/
    mkdir -p $PLUGINS_HOME/IKC-OP2
    cp -fv $PROJECT/Plugin_IKC-OP2/bin/Debug/IKC-OP2.dll $PLUGINS_HOME/IKC-OP2/
    mkdir -p $PLUGINS_HOME/MINI-FP6
    cp -fv $PROJECT/Plugin_MINI-FP6/bin/Debug/MINI-FP6.dll $PLUGINS_HOME/MINI-FP6/
    end "copy (with overwrite) plugins"
}

function _setBuildVersion() {
    #
    # set build version
    #
    VER="`getVersion`"
    chat "$VER"
    echo "$VER" > $APP_HOME/VERSION.txt
    echo "source:" >> $APP_HOME/VERSION.txt
    echo git branch >> $APP_HOME/VERSION.txt
    # generate github readme.md file
    echo "### PayDesk Solution" > $APP_HOME/README.md
    echo "-" >> $APP_HOME/README.md
    echo "##### Build Version: \`\`\` `getVersionNumber` \`\`\`" >> $APP_HOME/README.md

}

function _createAppPatch() {
    #
    # creating app patch archive
    #
    start "creating app patch archive"
    # echo arg is $1
    if ! [ "$1"="" ]
    then
        TYPE="$1_"
    fi


    ZIPNAME="../../AppUpdate_$TYPE`getVersionNumberFile`.7z"
    PASS=12345
    COMPRESSOR=./src/tools/compressor/7z.exe
    CARGS="a -t7z -p$PASS -mhe=on $APP_HOME/$ZIPNAME"
    CCMD="$COMPRESSOR $CARGS"

    (cd $APP_HOME/../../ && rm -rfv *{$1}*\.7z)
    
    $CCMD "$APP_HOME/*.bat"
    $CCMD "$APP_HOME/*.cfg"
    $CCMD "$APP_HOME/*.dll"
    $CCMD "$APP_HOME/*.DLL"
    $CCMD "$APP_HOME/*.exe"
    $CCMD "$APP_HOME/*.ico"
    $CCMD "$APP_HOME/*.ini"
    $CCMD "$APP_HOME/*.lic"
    $CCMD "$APP_HOME/*.txt"
    $CCMD "$APP_HOME/*.xml"

    $CCMD "$APP_HOME/display"
    $CCMD "$APP_HOME/manuals"
    $CCMD "$APP_HOME/Plugins"
    $CCMD "$APP_HOME/reports"
    $CCMD "$APP_HOME/schemes"
    $CCMD "$APP_HOME/templates"
    $CCMD "$APP_HOME/tools"
    $CCMD "$APP_HOME/users"
    
    end "creating app patch archive"
}

function _copyTools() {
    #
    # copying tools
    #
    start "copying tools"

    TOOL_CFG='<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <runtime>
    <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
      <probing privatePath="..\..\PayDesk\tools;bin;..\com" />
    </assemblyBinding>
  </runtime>
</configuration>'

    # BillsToExcel
    mkdir -pv $APP_HOME/tools/BillsToExcel/bin/
    cp -rfv $PROJECT/../PayDeskTools/BillsToExcel/bin/Debug/*.dll $APP_HOME/tools/BillsToExcel/bin/
    cp -rfv $PROJECT/../PayDeskTools/BillsToExcel/bin/Debug/*.exe $APP_HOME/tools/BillsToExcel/
    cp -rfv $PROJECT/../PayDeskTools/BillsToExcel/bin/Debug/*.bat $APP_HOME/tools/BillsToExcel/
    cp -rfv $PROJECT/../PayDeskTools/BillsToExcel/bin/Debug/config/ $APP_HOME/tools/BillsToExcel/
    echo $TOOL_CFG > $APP_HOME/tools/BillsToExcel/BillsToExcel.exe.config
    # ProductCategoryManager
    mkdir -pv $APP_HOME/tools/ProductCategoryManager/bin/
    cp -rfv $PROJECT/../PayDeskTools/ProductCategoryManager/bin/Debug/*.dll $APP_HOME/tools/ProductCategoryManager/bin/
    cp -rfv $PROJECT/../PayDeskTools/ProductCategoryManager/bin/Debug/*.exe $APP_HOME/tools/ProductCategoryManager/
    cp -rfv $PROJECT/../PayDeskTools/ProductCategoryManager/bin/Debug/*.bat $APP_HOME/tools/ProductCategoryManager/
    cp -rfv $PROJECT/../PayDeskTools/ProductCategoryManager/bin/Debug/config $APP_HOME/tools/ProductCategoryManager/
    echo $TOOL_CFG > $APP_HOME/tools/ProductCategoryManager/ProductCategoryManager.exe.config
    # VirtualKeyboard
    mkdir -pv $APP_HOME/tools/VirtualKeyboard/bin/
    cp -rfv $PROJECT/../PayDeskTools/VirtualKeyboard/bin/Debug/*.dll $APP_HOME/tools/VirtualKeyboard/bin/
    cp -rfv $PROJECT/../PayDeskTools/VirtualKeyboard/bin/Debug/*.exe $APP_HOME/tools/VirtualKeyboard/
    cp -rfv $PROJECT/../PayDeskTools/VirtualKeyboard/bin/Debug/*.bat $APP_HOME/tools/VirtualKeyboard/
    cp -rfv $PROJECT/../PayDeskTools/VirtualKeyboard/bin/Debug/config $APP_HOME/tools/VirtualKeyboard/
    echo $TOOL_CFG > $APP_HOME/tools/VirtualKeyboard/VirtualKeyboard.exe.config

    end "copying tools"
}