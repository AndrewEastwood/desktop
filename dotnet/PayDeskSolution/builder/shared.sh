#!/bin/bash

# DATAROOT="$( cd $( dirname "$0" ) && pwd )"
COLORSENABLED=true
PROJECT=./..
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
    echo "Build Version: `date +%y.%m%d`"
}

function build() {

    chat "build"
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

    #
    # copy (with overwrite) plugins
    #
    start "copy (with overwrite) plugins"
    cp -v $PROJECT/Plugin_DATECS_EXELLIO/bin/Debug/DATECS_EXELLIO.dll ./bin/Plugins/DATECS_EXELLIO/
    cp -v $PROJECT/Plugin_DATECS_FP3530T/bin/Debug/DATECS_FP3530T.dll ./bin/Plugins/DATECS_FP3530T/
    cp -v $PROJECT/Plugin_IKC_E260T/bin/Debug/IKC_E260T.dll ./bin/Plugins/IKC-E260T/
    end "copy (with overwrite) plugins"
    
    #
    # set build version
    #
    VER="`getVersion`"
    chat "$VER"
    echo "$VER" > ./bin/VERSION.txt
    

}