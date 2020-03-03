from functools import wraps

class Debugger(object):
    attribute_accesses = []
    method_calls = []

    @staticmethod
    def addMethodCall(cls, name, *args, **kwargs):
        Debugger.method_calls.append({"class": cls, "method": name, "args": args, "kwargs": kwargs})

    @staticmethod
    def addAttributeAccess(action, cls, attr, val = None):
        Debugger.attribute_accesses.append({"action": action, "class": cls, "attribute": attr, "value": val})

def debugFunc(cls):
    def debugFuncImpl(func):
        @wraps(func)
        def wrapper(*args, **kwargs):
            Debugger.addMethodCall(cls, func.__name__, *args, **kwargs)
            return func(*args, **kwargs)
        
        return wrapper
    return debugFuncImpl

def debugClass(cls, self):
    # attributes
    originalGetAttribute = cls.__getattribute__
    originalSetAttribute = cls.__setattr__
    
    def __getattribute__(self, name):
        val = originalGetAttribute(self, name)
        Debugger.addAttributeAccess("get", cls, name, val)
        return val

    def __setattr__(self, name, val):
        Debugger.addAttributeAccess("set", cls, name, val)
        originalSetAttribute(self, name, val)

    cls.__getattribute__ = __getattribute__
    cls.__setattr__ = __setattr__

    return cls

class Meta(type):
    def __new__(cls, name, bases, attrs, **extraKwargs):
        clsObj = super().__new__(cls, name, bases, attrs)
        clsObj = debugClass(clsObj)
        
        return clsObj
        
    def __init__(cls, *args, **kwargs):
        #init
        originalInit = cls.__init__
        def __init__(self, *args, **kwargs):
            for name, val in vars(cls).items():
                if callable(val) and name != "__init__":
                    setattr(self, name, debugFunc(cls, self)(val))

            originalInit(self, *args, **kwargs)

        cls.__init__ = __init__
