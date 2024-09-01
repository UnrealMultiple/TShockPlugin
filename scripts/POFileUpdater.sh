#!/bin/bash

shopt -s nullglob
for proj in $(dirname $0)/../src/*/*.csproj; do
  if [[ ! -d "$(dirname $proj)/i18n" ]]; then
    continue
  fi

  echo "[$proj] generating template.pot..."
  pushd $(dirname $proj) > /dev/null

  dotnet tool run GetText.Extractor -u -o -s ./ -t i18n/template.pot
  if [[ ! -f i18n/template.pot ]]; then
    popd > /dev/null
    continue 
  fi
  
  pot_diff=($(git diff --numstat HEAD -- i18n/template.pot))
  if [[ ${pot_diff[0]} == '2' ]] && [[ ${pot_diff[1]} == '2' ]]; then
    echo "[$proj] template.pot no diff except date changes, restoring..."
    git restore i18n/template.pot
#    popd > /dev/null
#    continue
  fi
  
  for pofile in ./i18n/*.po; do
    echo "[$pofile] merging..."
    msgmerge --previous --update $pofile i18n/template.pot
  done

  popd > /dev/null
done