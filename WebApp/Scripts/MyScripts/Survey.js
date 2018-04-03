    
var choicesCount = 1;
var questionCounter = 1;
var INPUT_CHOICE_ID = "choice";
var BUTTON_ADD_CHOICE_ID = "choiceadd";
var BUTTON_REM_CHOICE_ID = "choicerem";
var SINGLE_CHOICE = "single_choice";
var OPEN_TEXT = "open_text";
var MULTIPLE_CHOICE = "multiple_choice";
var surveyObject = [];

//o id choices sem numero está sempre disponivel não se pode apagar
var choicesIds = [];

function sendSurvey(id, token) {
    alert(JSON.stringify(surveyObject))
    if (surveyObject.length == 0)
        alert("To send  survey you have to had at least one question")
    else {
        $.ajax({
            url: 'https://exampleapp.com/api/events/'+id+'/survey',
            type: 'POST',
            headers: {
                "Authorization": "Bearer " + token,
            },
            data: JSON.stringify(surveyObject),
            contentType: 'application/json',
            success: function (data) {
                alert("Inserted");
            },
            error: function (error) {
                alert("Error" + JSON.stringify(error))
            }
        });
    }
    
}

$('#typeOfQuestion').on('change', function () {

    var value = this.value;
    var single = $(".single-aswer");
   
    if (value === 'Text') {
        single.addClass('hidden')
        removeElements();
        resetFirstChoice();
    }

    else {
        
        single.removeClass('hidden')
        
    }

});

function addQuestion() {

    $("#firstStep").hide();
    $('#sendSurvey').removeClass('hidden')
    $('#createQuestion').removeClass('hidden')
    
}


function addInputChoices() {
   
    $("#choices").append('<tr class="single-aswer form-group-sm"><td class="form-inline" ><input id=' + INPUT_CHOICE_ID + choicesCount + ' type="text"  class="form-control " value="New Choice"/><button id=' + BUTTON_ADD_CHOICE_ID + choicesCount + ' type="button" class="btn btn-success btn-xs" onclick="addInputChoices()"><i class="fa fa-plus"></i></button><button id= ' + BUTTON_REM_CHOICE_ID + choicesCount + ' type="button" class="btn btn-danger btn-xs" onclick="removeInputChoices(\'' +  choicesCount + '\')"><i class="fa fa-minus"></i></button></td></tr>');
    choicesIds.push(choicesCount);
    ++choicesCount;
}

function removeInputChoices(count) {
    
    $("#" + INPUT_CHOICE_ID + count).remove();
    $("#" + BUTTON_ADD_CHOICE_ID + count).remove();
    $("#" + BUTTON_REM_CHOICE_ID + count).remove();
}



function postQuestion() {
    var val = $('#typeOfQuestion').val();
    
    if (val === 'Text') {
        appendTextQuestion();
    }
    if (val === 'Single') {
        appendSingleQuestion();
        
        
    }
    if (val === 'Multiple') {
        appendMultipleQuestion();

    }
    resetQuestionEdit();
    resetFirstChoice();
    removeElements();
    $("#createQuestion").addClass('hidden');

}
function cancelQuestion() {

    $("#createQuestion").addClass('hidden');
    removeElements();
    resetFirstChoice();
    resetQuestionEdit();
   
}

function removeElements() {
    choicesIds.forEach(elem => removeInputChoices(elem))
    choicesIds = [];
}

function resetQuestionEdit() {
    $("#QuestionText").val("New Question")
}

function resetFirstChoice() {
    $("#"+ INPUT_CHOICE_ID).val("New Choice")
}

////// Funções para fazer append

function appendTextQuestion() {
    
    var q = appendQuestion();

    var res = { type: "open_text", question: q };
  
    $("#questions").append('<textarea  class="form-control" rows="3" cols="300" id="response1"></textarea>')
    surveyObject.push(res)
    ++questionCounter;
}

function appendSingleQuestion() {
    
    var q = appendQuestion();
    var res = { type: "single_choice", question: q, choices_messages: []  };
    

    var fistChoice = $("#" + INPUT_CHOICE_ID).val()
    res.choices_messages.push(fistChoice)
    
    createRadioButton(fistChoice)
    choicesIds.forEach(elem =>{
        var aux = $("#" + INPUT_CHOICE_ID + elem).val();
        createRadioButton(aux)
        res.choices_messages.push(aux);
    })

    surveyObject.push(res)
    ++questionCounter
}



function appendMultipleQuestion() {
    
    var q = appendQuestion();
    var res = { type: "multiple_choice", question: q, choices_messages: []  };

    var firstChoice = $("#" + INPUT_CHOICE_ID).val();
    res.choices_messages.push(firstChoice)

    createCheckBoxButton(firstChoice)
    choicesIds.forEach(elem =>{
        var aux = $("#" + INPUT_CHOICE_ID + elem).val();
        createCheckBoxButton(aux)
        res.choices_messages.push(aux);
    })
    surveyObject.push(res)
    ++questionCounter

}

function createRadioButton(value) {
    var q = $("#questions");
    var div = q.append('<div class="radio"><label><input name="optradio' + questionCounter + '" type="radio">' + value + '</label></div>')

}

function createCheckBoxButton(value) {

    var q = $("#questions");
    var div = q.append('<div class="checkbox"><label><input type="checkbox">' + value + '</label></div>')


}

function appendQuestion() {
    var text = $("#QuestionText").val();
    var q = $("#questions");
    q.append("<p>" + questionCounter + ". " + text + "</p>")
    return text;
}

/////////////////////////// Aswer analasys//////////////////////////////////////////////
var surveyAnswersRequested = [];
var PROGRESSBAR_COLOR = ["progress-bar-success", "progress-bar-info", "progress-bar-warning" ]
var open_text_answer = {};//2:{responses:[],current:0 }

function GetSurveyAnswers(token, id) {
    if (surveyAnswersRequested.length == 0) {

        Request(token, 'Get', {}, 'https://exampleapp.com/api/events/' + id + '/survey_answer', (data, error) => {
            if (data) {
                surveyAnswersRequested = data.questions;
                FillSurveyAnswer(data.questions)
            } else {
                alert(JSON.stringify(error))
            }

        })
    } 


};
function FillSurveyAnswer(data) {
    $("#WaitingAswers").addClass("hidden")

    surveyAnswersRequested.forEach(question => {
        let node = $("#SurveyAnswersDiv")
        let total = question.total;
        let id = question.id;
        node.append('<h4>' + question.id + '. ' + question.question + ' </h4> ')

        if (question.type === OPEN_TEXT) {
            open_text_answer[id] = {responses:question.response, current:0}
            node.append('<div class="row"><div class="col-md-6"><pre id="pre_' + id + '" >' + question.response[0] + '</pre><center><div class="btn btn-success btn-xs" onclick="PrevOpenTextAnswer(' + id + ')"><i class="fa fa-arrow-left"></i>Prev</div><div class="btn btn-success btn-xs" style="margin-left:10px" onclick="NextOpenTextAnswer(' + id + ')">Next<i class="fa fa-arrow-right"></i></div></center></div></div>')
           
        }
        else  {

            let counter = 0;
            question.response.forEach(res => {
                node.append(GetProgressBar(res.answer,res.count * 100 /total, counter))
                ++counter
                if (counter == 3)
                    counter = 0;
             
            })
        }
        
    })

}

function PrevOpenTextAnswer(id) {
    let q = open_text_answer[id];

    if (q.current != 0) 
        $("#pre_"+id).text(q.responses[--q.current])
    
}

function NextOpenTextAnswer(id){
    let q = open_text_answer[id];
    
    if(q.current != q.responses.length -1)
        $("#pre_"+id).text(q.responses[++q.current])
}

function GetProgressBar(text,value, color) {
    return '<div class="text-xs-center" >'+text+' ('+value+'%)</div> <div class="progress" style="height: 15px !important;"><div class="progress-bar ' + PROGRESSBAR_COLOR[color] + ' progress-bar-striped" role="progressbar"aria-valuenow="' + value + '" aria-valuemin="0" aria-valuemax="100" style="width:' + value + '%; font-size: 14px;"></div> </div>'    
}


///////////////////////////////////////////SendSurveyAnswer///////////////////////////////
function CheckAnswersOfSurvey(token, eventId, counter) {
    var surveyAnswer = []
    var all = true;
    for (var i = 1 ; i <= counter; ++i) {
        var type = $("#type_" + i).val()

        if (type === SINGLE_CHOICE) {
            var single = $('input[name="' + i + '"]:checked').val();
            if (single === undefined) {
                all = false;
                break;
            }
            var res = [];
            res.push(single);
            surveyAnswer.push({ questionId: i, choices: res })

        }
        else if (type === OPEN_TEXT) {
            var response = $("#" + i).val();
            if (response === "") {
                all = false;
                break;
            }
            surveyAnswer.push({ questionId: i, choices: [], text: response })
        }
        else if (type === MULTIPLE_CHOICE) {
            var items = [];
            $('input[name=' + i + ']:checked').each(function () { items.push($(this).val()); });

            if (items.length === 0) {
                all = false;
                break;
            }
            surveyAnswer.push({ questionId: i, choices: items })
        }

    }
    if (all) {
        sendSurveyAswer(token, eventId, surveyAnswer)
    }
    else {
        $("#sendSurveyWarning").append('<div class="row alert alert-danger alert-dismissible" role="alert"><button type="button" class="close" data-dismiss="alert"><span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>You must answer all questions to send the survey </div>');
    }
}

function sendSurveyAswer(token, eventId, surveyAnswer) {
    Request(token, 'Post', surveyAnswer, 'https://exampleapp.com/api/events/' + eventId + '/survey_answer', (data, error) => {
        if (data) {
            $("#sendSurveySuccess").append('<div class="row alert alert-success alert-dismissible" role="alert"><button type="button" class="close" data-dismiss="alert"><span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>Survey succefully send! Thanks for your time</div>');
        } else {
            $("#sendSurveyWarning").append('<div class="row alert alert-danger alert-dismissible" role="alert"><button type="button" class="close" data-dismiss="alert"><span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>' + error.responseJSON.detail + '</div>');

        }

    })

}
