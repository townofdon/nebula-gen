#!/bin/bash

source "./utils.sh"

#
# SCRIPT
#

assertFileExists "../Build/webgl/index.html"
rm -rf "./webgl"
mkdir -p "./webgl"
cp -a "../Build/webgl/." "./webgl"

docker-compose up
