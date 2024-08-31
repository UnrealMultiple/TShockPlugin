import glob
from pathlib import Path
import subprocess
import sys

def run_cmd(cmdline, is_output = False):
    p = subprocess.run(cmdline, capture_output=is_output)
    if is_output:
        out = p.stdout.decode()
        err = p.stderr.decode()
        if out != "" or err != "":
            print(out)
            print(err)

if __name__ == '__main__':
    args = sys.argv[1:]
    for csproj in [Path(x) for x in glob.glob(str(Path(sys.argv[0]).parent.parent) + '/src/*/*.csproj')]:
        i18n_dir = csproj.parent / 'i18n'
        if not i18n_dir.is_dir():
            if "auto" in args:
                if "debug" in args:
                    print(f"[{i18n_dir}] creating directory...")
                i18n_dir.mkdir()
            else:
                continue

        if "debug" in args:
            print(f"[{csproj}] generating template.pot...")

        run_cmd(["dotnet", "tool", "run", "GetText.Extractor", "-u", "-o", "-s", str(csproj), "-t", str(i18n_dir / "template.pot")], "debug" in args)

        for po_file in [Path(x) for x in glob.glob(i18n_dir.as_posix() + '/*.po')]:
            if "debug" in args:
                print(f"[{po_file}] merging...")
            subprocess.run(["msgmerge", "--previous", "--update", str(po_file), str(i18n_dir / "template.pot")], "debug" in args)