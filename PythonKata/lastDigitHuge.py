
def last_digit(lst):
    val = 1
    for v in reversed(lst):
        val = lastNDigits(v, val, 3)
        if val == 0 and v > 0: val = 100
    
    return val % 10

#Euler Theorem
#last n digits of x ** y = phin(n)
#this is not actually used in this problem because it is alway last digit
#for that I need phi(100) which is 40 - use it as a constant
def phi(n): 
    result = n 
    p = 2
    while(p * p <= n): 
        if (n % p == 0): 
            while (n % p == 0): 
                n = n // p 
            result = result * (1.0 - (1.0 / (float) (p))) 
        p = p + 1

    if (n > 1): 
        result = result * (1.0 - (1.0 / (float)(n))) 
   
    return (int)(result) 

def lastDigit(n1, n2):
    return (n1 % 10) ** (n2 % 4 + 4 * bool(n2)) % 10

def lastNDigits(n1, n2, n):
    pow10n = 10 ** n
    return (n1 % pow10n) ** (n2 % phi(pow10n)) % pow10n
