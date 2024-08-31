import glob
from pathlib import Path
import subprocess
import sys

if __name__ == '__main__':
    model = "auto"
    for csproj in [Path(x) for x in glob.glob(str(Path(sys.argv[0]).parent.parent) + '/src/*/*.csproj')]:
        i18n_dir = csproj.parent / 'i18n'
        if not i18n_dir.is_dir():
            if model == "auto":
                i18n_dir.mkdir()
            else:
                continue

        subprocess.run("dotnet tool run GetText.Extractor -s " + str(csproj) + " -t " + str(i18n_dir / "template.pot"))


        for po_file in [Path(x) for x in glob.glob(i18n_dir.as_posix() + '/*.po')]:
            subprocess.run(["msgmerge", "--previous", "--update", str(po_file), str(i18n_dir / "template.pot")])