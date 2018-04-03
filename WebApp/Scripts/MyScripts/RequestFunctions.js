function PutSubscriber(token, eventId, callback) {
    if (token == "none") window.location.href = 'https://exampleapp.com/Account/Login';

    else
        $.ajax({
            url: 'https://exampleapp.com/api/events/'+eventId+'/subscribe',
            type: 'PUT',
            headers: {
                "Authorization": "Bearer " + token,
            },
            success: function (data) {
                callback()
            },
            error: function (error) {
                alert("Error" + JSON.stringify(error))
                callback(JSON.stringify(error))
            }
        });
}

function RequestGet() { }

var newToken=null;

function Request(token, method, body, url, callBack) {

    if (newToken != null) token = newToken;
   
    $.ajax({
        url: url,
        type: method,
        headers: {
            "Authorization": "Bearer " + token,
        },
        data: JSON.stringify(body),
        contentType: 'application/json',
        success: function (data) {
            callBack(data)
        },
        error: function (error) {
            refreshToken(error, (success, result) => {
                if (success) {
                    newToken = result;
                    Request(result, method, body, url, callBack)
                }
                else {
                    callBack(null, error) //+ "  <-Then->  " + result)
                }
            });
        }
    });
}

function refreshToken(error, callback) {

    if (error.status == 401) {

        $.ajax({
            url: 'https://exampleapp.com/Helper/Refresh',
            type: 'GET',
            success: function (data) {
                alert("token retriever: " + data)
                callback(true, data)
            },
            error: function (error) {
                alert("error on geting new token: " + JSON.stringify(error))
                callback(false, error)
            }
        });

    } else {
        callback(false);
    }
}

