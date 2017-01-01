#!/bin/bash

# see https://fozztech.wordpress.com/2015/06/04/using-monodevelop-to-develop-scripts-in-space-engineers/

if [[ -d ".git-temp" ]]; then
    cd .git-temp
    git checkout .
    git pull
else
    git clone https://github.com/KeenSoftwareHouse/SpaceEngineers.git .git-temp
    cd .git-temp
fi

function sync_source {
	rsync -a --include "*.cs" "./Sources/${1}" "./../Stubs/${1}" && echo "Synchronized ${1}" || echo "Synchronization of ${1} failed"
}

sync_source "Sandbox.Common"
sync_source "VRage.Library"
sync_source "VRage.Math"
sync_source "VRage"
