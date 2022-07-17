mergeInto(LibraryManager.library, {

  Hello: function () {
    window.alert("Hello, world!");
  },

  IsMobile: function()
  {
    return UnityLoader.SystemInfo.mobile;
  },

  Redirect: function (str) {
    window.location.href = Pointer_stringify(str);
  },

  HelloString: function (str) {
    window.alert(Pointer_stringify(str));
  }

});