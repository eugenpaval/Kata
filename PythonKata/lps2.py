def longest_palindrome2(s):
    max_, l_ = '', len(s)
    if len(set(s))==1:
        return s

    A = lambda s,l,r:\
        A(s,l-1,r+1) if l-1>=0 and r+1<l_ and s[l-1]==s[r+1] else s[l:r+1]    
    
    for i in range(l_):\
        t, t1 = A(s,i,i), A(s,i,i+1) if i+1<l_ and s[i]==s[i+1] else ''
    
    max_ = max(max_,t,t1,key=len)
    
    return max_ 