import glob
from pathlib import Path
import subprocess
import sys

def run_cmd(cmdline, capture_output = False):
    p = subprocess.run(cmdline, capture_output=capture_output)
    if capture_output:
        return p.stdout.decode()
    return None

if __name__ == '__main__':
    args = sys.argv[1:]
    for csproj in [Path(x) for x in glob.glob(str(Path(sys.argv[0]).parent.parent) + '/src/*/*.csproj')]:
        i18n_dir = csproj.parent / 'i18n'
        if not i18n_dir.is_dir():
            if "auto" in args:
                print(f"[{i18n_dir}] creating directory...")
                i18n_dir.mkdir()
            else:
                continue
                
        print(f"[{csproj}] generating template.pot...")
        pot_file = i18n_dir / "template.pot"
        run_cmd(["dotnet", "tool", "run", "GetText.Extractor", "-u", "-o", "-s", str(csproj), "-t", str(pot_file)])

        pot_diff = [x for x in run_cmd(["git", "diff", "--numstat", str(pot_file)], True).split(' ') if x != ""]
        if pot_diff[0] == "2" and pot_diff[1] == "2":
            f"[{str(csproj)}] template.pot no diff except date changes, restoring..."
            run_cmd(["git", "restore", str(pot_file)])
            continue

        for po_file in [Path(x) for x in glob.glob(i18n_dir.as_posix() + '/*.po')]:
            print(f"[{po_file}] merging...")
            run_cmd(["msgmerge", "--previous", "--update", str(po_file), str(i18n_dir / "template.pot")])
