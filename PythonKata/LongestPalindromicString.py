# Python program to implement Manacher's Algorithm 

def longest_palindrome(text):
	positions = len(text) 
	
	if positions == 0: 
		return ''

	positions = 2*positions+1 # Position count 

	lengths = [0] * positions 
	lengths[0] = 0
	lengths[1] = 1

	centerPos = 1	 # centerPosition 
	centerRightPos = 2	 # centerRightPosition 
	currentLeftPos = 0	 # currentLeftPosition 
	
	maxLPSLength = 0
	maxLPSCenterPosition = 0
	
	start = -1
	end = -1
	diff = -1

	for i in range(1,positions): 
		currentLeftPos = 2*centerPos-i 
		lengths[i] = 0
		diff = centerRightPos - i 

		# If currentRightPos is within centerRightPos
		if diff > 0: 
			lengths[i] = min(lengths[currentLeftPos], diff) 

		try: 
			while ((i + lengths[i]) < positions and (i - lengths[i]) > 0) and (((i + lengths[i] + 1) % 2 == 0) or (text[int((i + lengths[i] + 1) / 2)] == text[int((i - lengths[i] - 1) / 2)])): 
				lengths[i]+=1
		except Exception as e: 
			pass

		if lengths[i] > maxLPSLength:	 # Track maxLPSLength 
			maxLPSLength = lengths[i] 
			maxLPSCenterPosition = i 

		# If palindrome centered at currentRightPos
		# expand beyond centerRightPos, 
		# adjust centerPos based on expanded palindrome. 
		if i + lengths[i] > centerRightPos: 
			centerPos = i 
			centerRightPos = i + lengths[i] 

	start = int((maxLPSCenterPosition - maxLPSLength) / 2)
	end = int(start + maxLPSLength - 1)
	end = 0 if end == -1 else end

	return text[start : end + 1]
