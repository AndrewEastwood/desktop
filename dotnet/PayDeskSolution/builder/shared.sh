#!/bin/bash

# DATAROOT="$( cd $( dirname "$0" ) && pwd )"
COLORSENABLED=true
PROJECT=./../
DEBUG=./../Engine/bin/Debug

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
    rm -rf ./bin/*
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
    cp -v $DEBUG/PayDesk.exe ./bin/
    cp -v $DEBUG/driver.dll ./bin/
    cp -v $DEBUG/components.dll ./bin/
    cp -v $DEBUG/HtmlAgilityPack.dll ./bin/
    cp -v $DEBUG/Ionic.Zip.dll ./bin/
    cp -v $DEBUG/Mono.Security.dll ./bin/
    cp -v $DEBUG/Npgsql.dll ./bin/
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
            cp -rf "$f" ./bin/
        elif [ -f "$f" ]
        then
            cp -rfv "$f" ./bin/
        fi
    done
    # cp -rv ./src/* ./bin/
    end "copy source"
}

function _cleanGeneratedData() {
    #
    # clean generated data
    #
    start "clean generated data"
    rm -rfv ./bin/articles/*
    rm -rfv ./bin/bills/*
    rm -rfv ./bin/cheques/*
    rm -rfv ./bin/reports/*
    end "clean generated data"
}

function _updatePlugins() {
    #
    # copy (with overwrite) plugins
    #
    PLUGINS_HOME=./bin/Plugins
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
    echo "$VER" > ./bin/VERSION.txt
    # generate github readme.md file
    echo "### PayDesk Solution" > ./bin/README.md
    echo "-" >> ./bin/README.md
    echo "##### Build Version: \`\`\` `getVersionNumber` \`\`\`" >> ./bin/README.md

}

function _createAppPatch() {
    #
    # creating app patch archive
    #
    start "creating app patch archive"
    # echo arg is $1
    if ! "$1"=""
    then
        TYPE="$1_"
    fi


    ZIPNAME="../../AppUpdate_$TYPE`getVersionNumberFile`.7z"
    PASS=12345
    COMPRESSOR=./src/tools/compressor/7z.exe
    CARGS="a -t7z -p$PASS -mhe=on ./bin/$ZIPNAME"
    CCMD="$COMPRESSOR $CARGS"

    (cd ./bin/../../ && rm -rfv *{$1}*.7z)
    
    $CCMD "./bin/*.bat"
    $CCMD "./bin/*.cfg"
    $CCMD "./bin/*.dll"
    $CCMD "./bin/*.DLL"
    $CCMD "./bin/*.exe"
    $CCMD "./bin/*.ico"
    $CCMD "./bin/*.ini"
    $CCMD "./bin/*.lic"
    $CCMD "./bin/*.txt"
    $CCMD "./bin/*.xml"

    $CCMD "./bin/display"
    $CCMD "./bin/manuals"
    $CCMD "./bin/Plugins"
    $CCMD "./bin/reports"
    $CCMD "./bin/schemes"
    $CCMD "./bin/templates"
    $CCMD "./bin/tools"
    $CCMD "./bin/users"
    
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
    mkdir -pv ./bin/tools/BillsToExcel/bin/
    cp -rfv $PROJECT/../PayDeskTools/BillsToExcel/bin/Debug/*.dll ./bin/tools/BillsToExcel/bin/
    cp -rfv $PROJECT/../PayDeskTools/BillsToExcel/bin/Debug/*.exe ./bin/tools/BillsToExcel/
    cp -rfv $PROJECT/../PayDeskTools/BillsToExcel/bin/Debug/*.bat ./bin/tools/BillsToExcel/
    cp -rfv $PROJECT/../PayDeskTools/BillsToExcel/bin/Debug/config/ ./bin/tools/BillsToExcel/
    echo $TOOL_CFG > ./bin/tools/BillsToExcel/BillsToExcel.exe.config
    # ProductCategoryManager
    mkdir -pv ./bin/tools/ProductCategoryManager/bin/
    cp -rfv $PROJECT/../PayDeskTools/ProductCategoryManager/bin/Debug/*.dll ./bin/tools/ProductCategoryManager/bin/
    cp -rfv $PROJECT/../PayDeskTools/ProductCategoryManager/bin/Debug/*.exe ./bin/tools/ProductCategoryManager/
    cp -rfv $PROJECT/../PayDeskTools/ProductCategoryManager/bin/Debug/*.bat ./bin/tools/ProductCategoryManager/
    cp -rfv $PROJECT/../PayDeskTools/ProductCategoryManager/bin/Debug/config ./bin/tools/ProductCategoryManager/
    echo $TOOL_CFG > ./bin/tools/ProductCategoryManager/ProductCategoryManager.exe.config
    # VirtualKeyboard
    mkdir -pv ./bin/tools/VirtualKeyboard/bin/
    cp -rfv $PROJECT/../PayDeskTools/VirtualKeyboard/bin/Debug/*.dll ./bin/tools/VirtualKeyboard/bin/
    cp -rfv $PROJECT/../PayDeskTools/VirtualKeyboard/bin/Debug/*.exe ./bin/tools/VirtualKeyboard/
    cp -rfv $PROJECT/../PayDeskTools/VirtualKeyboard/bin/Debug/*.bat ./bin/tools/VirtualKeyboard/
    cp -rfv $PROJECT/../PayDeskTools/VirtualKeyboard/bin/Debug/config ./bin/tools/VirtualKeyboard/
    echo $TOOL_CFG > ./bin/tools/VirtualKeyboard/VirtualKeyboard.exe.config

    end "copying tools"
}