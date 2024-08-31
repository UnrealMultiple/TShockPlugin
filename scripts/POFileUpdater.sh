#!/bin/bash

shopt -s nullglob
for proj in $(dirname $0)/../src/*/*.csproj; do
  if [ ! -d "$(dirname $proj)/i18n" ]; then
    continue
  fi

  pushd $(dirname $proj) > /dev/null

  dotnet tool run GetText.Extractor -s ./ -t i18n/template.pot
  
  for pofile in ./i18n/*.po; do
    msgmerge --previous --update $pofile i18n/template.pot
  done

  popd > /dev/null
done