#!/bin/bash

for proj in ./*/*.csproj; do
  if [ ! -d "$(dirname $proj)/i18n" ]; then
    continue
  fi

  pushd $(dirname $proj)

  GetText.Extractor -s ./ -t i18n/template.pot
  
  for pofile in ./i18n/*.po; do
    msgmerge --previous --update $pofile i18n/template.pot
  done

  popd
done