def format_duration(seconds):
    if seconds == 0:
        return "now"

    Minute : int = 60;
    Hour : int = Minute * 60;
    Day : int = Hour * 24;
    Year : int = Day * 365;

    y : int = int(seconds / Year);
    seconds -= y * Year;
    d : int = int(seconds / Day);
    seconds -= d * Day;
    h : int = int(seconds / Hour);
    seconds -= h * Hour;
    m : int = int(seconds / Minute);
    seconds -= m * Minute;

    outStringList = list(filter(lambda s: s != None, [output(y, "year"), output(d, "day"), output(h, "hour"), output(m, "minute"), output(seconds, "second")]))
    outString = ""
    l = len(outStringList)

    for i in range(0, l):
        outString += ", " if i > 0 and i < l - 1 else " and " if i > 0 and i == l - 1 else ""
        outString += outStringList[i]

    return outString

def plural(x : int):
    return "s" if x > 1 else ""

def output(x : int, s):
    return str(x) + " " + s + plural(x) if x > 0 else None

