#!/bin/bash

source "./utils.sh"

GAME="nebula-gen"
CHANNEL="html"
IS_MAC_OS=true

#
# COLORS
# see: https://stackoverflow.com/questions/5947742/how-to-change-the-output-color-of-echo-in-linux
#
NC='\033[0m'              # No Color
BLACK='\033[0;30m'        # Black
RED='\033[0;31m'          # Red
GREEN='\033[0;32m'        # Green
YELLOW='\033[0;33m'       # Yellow
BLUE='\033[0;34m'         # Blue
PURPLE='\033[0;35m'       # Purple
CYAN='\033[0;36m'         # Cyan
WHITE='\033[0;37m'        # White
GREY='\033[1;30m'         # Grey

#
# GET USERNAME
#
USERNAME=$(readEnvVar ITCHIO_USERNAME)
assertVarExists $USERNAME

#
# GET VERSION
#
VERSION=$(cat version.json \
  | grep version \
  | head -1 \
  | awk -F: '{ print $2 }' \
  | sed 's/[",]//g' \
  | tr -d '[[:space:]]')
SAFE_VERSION="${VERSION//./$'-'}"

#
# SCRIPT
#

info "WELCOME TO THE UNITY DEPLOYMENT SCRIPT!"
info "USER=${YELLOW}${USERNAME}"
info "GAME=${YELLOW}${GAME}"
info "About to push version ${RED}${VERSION}${CYAN} - proceed?"
prompt "(y/n)"

assertFileExists "../Build/webgl/index.html"
mkdir -p "../Archives"

# zip
if $IS_MAC_OS
then
  cd "../"
  ZIPFILE="build-${SAFE_VERSION}.zip"
  log "creating zip archive for ${ZIPFILE}..."
  zip -rq $ZIPFILE "./Build/webgl"
  assertFileExists $ZIPFILE
  mv $ZIPFILE "./Archives/$ZIPFILE"
  cd "./DeployScripts"
  ZIPFILE="../Archives/$ZIPFILE"
else
  ZIPFILE="../Archives/build-${SAFE_VERSION}.zip"
  log "creating zip archive for ${ZIPFILE}..."
  7z a $ZIPFILE "../Build/webgl" > NUL
  assertFileExists $ZIPFILE
fi


log "deploying to itch.io..."

# push to itch.io
butler push $ZIPFILE "${USERNAME}/${GAME}:${CHANNEL}" --userversion $VERSION

rm -rf "./NUL"
rm -rf "../NUL"
success "All done!"
