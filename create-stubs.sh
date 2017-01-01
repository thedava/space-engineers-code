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

rsync -a --include "*.cs" "./Sources/Sandbox.Common/ModAPI/" "./stub/Sandbox.Common"
rsync -a --include "*.cs" "./Sources/VRage.Library/" "./stub/VRage.Library"
rsync -a --include "*.cs" "./Sources/VRage.Math/" "./stub/VRage.Math"
rsync -a --include "*.cs" "./Sources/VRage/" "./stub/VRage"

