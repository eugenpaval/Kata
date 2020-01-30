function primeFactors(n) {
    const result = new Array();
    var p = 1;

    let i = 0;
    for (; n % 2 == 0;) {
        result[i] = { prime: 2, power: p++ };
        n /= 2;
    }

    i = result.length;
    for (let j = 3; j < Math.sqrt(n) + 1; j += 2) {
        let m = n % j;

        if (m == 0) {
            p = 1;

            while (m == 0) {
                result[i] = { prime: j, power: p++ };
                n /= j;
                m = n % j;
            }
            ++i;
        }
    }

    if (n > 2) result.push({ prime: n, power: 1 });

    var retVal = "";
    for (i = 0; i < result.length; ++i) {
        retVal += `(${result[i].prime}${result[i].power > 1 ? `**${result[i].power}` : ""})`;
    }

    return retVal;
}
