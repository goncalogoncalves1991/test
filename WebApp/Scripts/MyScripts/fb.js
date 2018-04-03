 

  function statusChangeCallback(response,url,callback) {

      if (response.status === 'connected') {
          return testAPI(url,callback);
      } else if (response.status === 'not_authorized') {
          callback("Please log into this app.",null);
      } else {
          callback("Please log into Facebook.", null);
      }
  }


  function checkLoginState(url,callback) {
      FB.getLoginStatus(function (response) {
          statusChangeCallback(response,url,callback);
      });
  }

  window.fbAsyncInit = function () {
      FB.init({
          appId: '1738062506473012',
          cookie: true,  // enable cookies to allow the server to access 
          // the session
          xfbml: true,  // parse social plugins on this page
          version: 'v2.5' // use graph api version 2.5
      });

  };

  // Load the SDK asynchronously
  (function (d, s, id) {
      var js, fjs = d.getElementsByTagName(s)[0];
      if (d.getElementById(id)) return;
      js = d.createElement(s); js.id = id;
      js.src = "//connect.facebook.net/en_US/sdk.js#xfbml=1&version=v2.7";
      //js.src = "//connect.facebook.net/en_US/sdk.js";
      fjs.parentNode.insertBefore(js, fjs);
  }(document, 'script', 'facebook-jssdk'));

  

  function testAPI(url,callback) {

      FB.api(url+'?fields=access_token', 'GET', null, function (response) {
          //alert('Successful login for: ' + response.access_token);
          //alert('Successful login for: ' + response.id);
          //alert(JSON.stringify(response));
          if (response.error != undefined) callback("unable to get this facebook page: https://www.facebook.com/"+url);
          else {             
              callback(null, response);
          }

      });
      
  }

  function fbtest() {
      FB.api('/eventcommittest/feed', 'post', { message: "message", access_token: 'EAAYswjZALtjQBAMM0hS7YYDw7DfQ3wZBJQa6ZBZB86aiqx8DluOXxBk4s5vdHubWBKa2f3Tqc2XXUyZAWDMZCIk1H4m1XeIIcZBVGSKBum8dKW0ojHC79C4FmIV8r4loA32jFNZC08Izd798t4LAePHlxKIhZAz26s8RbplgK2pIc7s2wHAUGHZCKH' }, function (response) {
          alert(JSON.stringify(response));
          if (!response || response.error) {
              alert('Error occured');
          } else {
              alert('Post ID: ' + response.id);
          }
      });
  }
