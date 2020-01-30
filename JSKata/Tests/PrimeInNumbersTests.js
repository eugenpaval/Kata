import {assert} from "assert.js";
import {primes} from "../PrimeInNumbers.js";

describe("Test Suite 1", function ()
{
    it
    (
        "Test 1", function () {
            assert.equal(primes(7775460), "(2**2)(3**3)(5)(7)(11**2)(17)");
        }
    );
});
