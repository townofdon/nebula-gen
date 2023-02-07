#
# UTILS
#
log() {
  echo -e "${NC}${1}${NC}"
}
info() {
  echo -e "${CYAN}${1}${NC}"
}
success() {
  echo -e "${GREEN}✓ ${1}${NC}"
}
warn() {
  echo -e "${YELLOW}${1}${NC}"
}
error() {
  echo -e "${RED}${1}${NC}"
}
prompt() {
  read -p "$1 " -n 1 -r
  echo    # (optional) move to a new line
  if [[ ! $REPLY =~ ^[Yy]$ ]]
  then
      warn "user cancelled."
      exit 1
  fi
}
assertFileExists() {
  if [ ! -f "$1" ]; then
      error "$1 does not exist."
      exit 1
  fi
}
assertDirExists() {
  if [ ! -d "$1" ]; then
      error "$1 does not exist."
      exit 1
  fi
}
assertVarExists() {
  if [ -z "$1" ]; then
      error "Var does not exist"
      exit 1
  fi
}
readEnvVar() {
  VAR=$(grep "^$1=" "../.env" | xargs)
  IFS="=" read -ra VAR <<< "$VAR"
  IFS=" "
  echo ${VAR[1]}
}