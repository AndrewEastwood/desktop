#!/bin/bash

. ./shared.sh

function main() {
    cleanup
    build
    if [ "$1" = "start" ]
    then
        cd ./bin/
        start PayDesk.exe
        ./PayDesk.exe &
    fi
    if [ "$1" = "clean" ]
    then
        _cleanGeneratedData
    fi
}

time main $@