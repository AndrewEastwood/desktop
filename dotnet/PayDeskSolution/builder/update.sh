#!/bin/bash

. ./shared.sh

function main() {
    _updateLibs
    _updatePlugins
    _setBuildVersion
}

time main