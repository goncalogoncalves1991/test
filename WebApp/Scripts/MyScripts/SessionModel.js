var sessionId;
var token;
var userId;
function GetSession(id, t,user) {
    $.ajax({
        url: 'https://exampleapp.com/api/sessions/' + id ,
        type: 'Get',

        success: function (data) {
            //alert(JSON.stringify(data))
            sessionId = data.id
            token = t;
            userId = user;
            CompleteSessionModel(data)
        },
        error: function (error) {
            alert("Error" + JSON.stringify(error))
        }
    });
}


function CompleteSessionModel(data) {
    $("#SessionInfo").removeClass("hidden")
    $("#loading").addClass("hidden")

    $("#session-title").text(data.title)
    $("#session-description").text(data.description)
    $("#session-initdate").text(data.initialDate)
    $("#session-enddate").text(data.endDate)
    $("#session-speakername").text(data.speaker)
    $("#session-linkspeaker").text(data.profileSpeaker)
    // Só completo este modelo caso seja um utilizador loggado
    //se é um utilizador autenticado
    if(token != "none" )
        VerifyIsUserCheckedInOnEvent(data.questions)
}

function VerifyIsUserCheckedInOnEvent(questions) {
    //alert("ola " +token)
    $.ajax({
        url: 'https://exampleapp.com/api/events/1/isSubscribed',
        type: 'Get',
        headers: {
            "Authorization": "Bearer " + token,
        },
        success: function (data) {
            //alert(JSON.stringify(data))
            if (data.subscribed) {
                CompleteQuestionsModel(questions)
            }  
            else {
                $("#question-list").append('<div class=" question-remove row alert alert-danger alert-dismissible" role="alert"><button type="button" class="close" data-dismiss="alert"><span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>You have to be checked-In on event to can see and make questions</div>')
            }
        },
        error: function (error) {
            alert("Error" + JSON.stringify(error))
        }
    });
}
function CompleteQuestionsModel(questions) {
    $("#insertQuestion").append('<center><h3 class="question-remove">Questions</h3></center><div class="comment-avatar question-remove"><img src="http://i9.photobucket.com/albums/a88/creaticode/avatar_2_zps7de12f8b.jpg" alt=""></div><div class="form-inline question-remove"><div class="form-group"><textarea id="questionInput" class="form-control question-remove" rows="2" cols="100"></textarea><button onclick="PostQuestion()" class="btn  btn-xs btn-success question-remove"><i class="fa fa-send"> Add</i></button></div></div><br />')

    $("#questions").removeClass('hidden')
    questions.forEach(function (q) {
        if (q.likers.indexOf(userId) == -1)
            $("#question-list").append('<div class="question-remove"><div class="comment-avatar"><img src="http://i9.photobucket.com/albums/a88/creaticode/avatar_2_zps7de12f8b.jpg" alt=""></div><div class="panel panel-default"><div class="panel-heading" style="padding-left:5em">' + q.authorName + '<button onclick="Like(this,' + q.id + ')" class=" btn btn-xs btn-facebook pull-right"><i class="fa fa-thumbs-o-up"></i>Like<span style="padding-left:1em" id="' + q.id + '">' + q.likes + '</span></button></div><div class="panel-body" style="padding-left:5em">' + q.question + '</div></div></div>')
        else
            $("#question-list").append('<div class="question-remove"><div class="comment-avatar"><img src="http://i9.photobucket.com/albums/a88/creaticode/avatar_2_zps7de12f8b.jpg" alt=""></div><div class="panel panel-default"><div class="panel-heading" style="padding-left:5em">' + q.authorName + '<button  disabled onclick="Like(this,' + q.id + ')" class=" btn btn-xs btn-facebook pull-right"><i class="fa fa-thumbs-o-up"></i>Like<span style="padding-left:1em" id="' + q.id + '">' + q.likes + '</span></button></div><div class="panel-body" style="padding-left:5em">' + q.question + '</div></div></div>')
    })
}

function Like(button,id) {
    
    $.ajax({
        url: 'https://exampleapp.com/api/questions/'+id+'/like',
        type: 'POST',
        headers: {
            "Authorization": "Bearer " + token,
        },
        success: function (data) {
            $("#" + id).text(parseInt($("#" + id).text()) + 1)
            button.disabled = true;
        },
        error: function (error) {
            alert("Error" + JSON.stringify(error))
        }
    });
}


function PostQuestion() {
    var input = $("#questionInput").val()
    if(input == "")alert("You have to write something...")
    else {
        var send = {question: input}
        $.ajax({
            url: 'https://exampleapp.com/api/sessions/'+sessionId+'/questions',
            type: 'POST',
            headers: {
                "Authorization": "Bearer " + token,
            },
            data: JSON.stringify(send),
            contentType: 'application/json',
            success: function (data) {
                alert("New question add" );
                $("#question-list").prepend('<div class="question-remove"><div class="comment-avatar"><img src="http://i9.photobucket.com/albums/a88/creaticode/avatar_2_zps7de12f8b.jpg" alt=""></div><div class="panel panel-default"><div class="panel-heading" style="padding-left:5em">Está hardcoded<button onclick="Like(this,' + data.id + ')" class=" btn btn-xs btn-facebook pull-right"><i class="fa fa-thumbs-o-up"></i>Like<span style="padding-left:1em" id="' + data.id + '">0</span></button></div><div class="panel-body" style="padding-left:5em">' + input + '</div></div></div>')
                $("#questionInput").val("")
            },
            error: function (error) {
                alert("Error" + JSON.stringify(error))
                $("#questionInput").val("") 
            }
        });

    }
        
}

function ClearSessionModal() {
    $("#questionInput").val("")
    $(".question-remove").remove();
    $("#SessionInfo").addClass("hidden")
    $("#loading").removeClass("hidden")
}