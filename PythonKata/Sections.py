def c(k):
	xy = k ** 0.5
	xyint = int(xy)

	if xy != xyint:
		return 0

	factors = primeFactors(xyint)

	count = 1
	for f in factors:
		count *= f[1] * 3 + 1

	return count

def primeFactors(n): 
	factors = [[0] * 2]
	r = 0
	
	c = 1
	while n % 2 == 0: 
		factors[r] = [2,c]
		n = n / 2
		c += 1
		
	# n must be odd at this point 
	# so a skip of 2 ( i = i + 2) can be used 
	for i in range(3, int(n ** 0.5) + 1, 2): 
		# while i divides n , print i ad divide n 
		c = 1

		f = n % i
		if f == 0:
			factors.append([i,c])
			r += 1
			while f == 0: 
				factors[r][1] = c
				c += 1 
				n = n / i 
				f = n % i
			
	# Condition if n is a prime 
	# number greater than 2 
	if n > 2: 
		factors.append([int(n),1])

	return factors