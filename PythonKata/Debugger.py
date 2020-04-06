from functools import wraps

class Debugger(object):
    attribute_accesses = []
    method_calls = []

    @staticmethod
    def addMethodCall(cls, name, args, kwargs):
        Debugger.method_calls.append({"class": cls, "method": name, "args": args, "kwargs": kwargs})

    @staticmethod
    def addAttributeAccess(action, cls, attr, val = None):
        Debugger.attribute_accesses.append({"action": action, "class": cls, "attribute": attr, "value": val})

def debugFunc(cls):
    def debugFuncImpl(func, self):
        @wraps(func)
        def wrapper(*args, **kwargs):
            Debugger.addMethodCall(cls, func.__name__, (self,) + args, kwargs)
            return func(self, *args, **kwargs)
        
        return wrapper
    return debugFuncImpl

<<<<<<< HEAD
def debugClass(cls, self):
=======
def debugClass(cls):
>>>>>>> 9f08d763222d2b6944fbfe911130cb83d6150d8a
    # attributes
    originalGetAttribute = cls.__getattribute__
    originalSetAttribute = cls.__setattr__
    
    def __getattribute__(self, name):
        val = originalGetAttribute(self, name)
        
        if name[0:2] != "__" and name[:-2] != "__":
            Debugger.addAttributeAccess("get", cls, name, val)

        return val

    def __setattr__(self, name, val):
        if not callable(val):
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

<<<<<<< HEAD
=======
    def __init__(cls, *args, **kwargs):
        originalInit = cls.__init__

        def __init__(self, *args, **kwargs):
            originalInit(self, *args, **kwargs)
            Debugger.addMethodCall(cls, "__init__", (self,) + args, kwargs)

            for name, val in vars(cls).items():
                if callable(val) and name not in ["__init__", "__getattribute__", "__setattr__"]:
                    setattr(self, name, debugFunc(cls)(val, self))

>>>>>>> 9f08d763222d2b6944fbfe911130cb83d6150d8a
        cls.__init__ = __init__
