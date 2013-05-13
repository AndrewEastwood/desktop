#!/bin/bash

. ./shared.sh

function main() {
    cleanup
    build
    start ./bin/PayDesk.exe &
    ./bin/PayDesk.exe &
}

time main