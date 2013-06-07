#!/bin/bash

. ./shared.sh

function main() {
    cleanup
    if [ "$1" = "test" ]
    then
        buildTest
    fi
    if [ "$1" = "prod" ]
    then
        buildProduction
    fi
    if [ "$2" = "start" ]
    then
        cd ./bin/
        start PayDesk.exe
        ./PayDesk.exe &
    fi
}

time main $@