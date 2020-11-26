function alertFunction() {
    alert(new Date().toString());
}

function promptFunction() {
    var person = prompt("Please enter your name", "John Doe");
    if (person != null) {
        document.getElementById("nameParagraphId").innerHTML =
        "Hello " + person + "! How are you today?";
    }
}

function showDiv() {
    document.getElementById('hiddenDivId').style.display = 'block';
}

function hideDiv() {
    document.getElementById('hiddenDivId').style.display = 'none';
}

function processForm1() {
    //let x = document.forms["testForm1"]["txtBoxTestForm1Id"].value;
	let x = document.forms["testForm1"]["selectListDrowDownID"].value;
    if (x != null && x != "") {
        alert(x);
        return false;
    } else {
		alert("name field was empty");
		return false;
	}
}

function disableSubmitButton() {
    document.getElementById("btnSubmitTestForm1Id").disabled = true;
}

function enableSubmitButton() {
    document.getElementById("btnSubmitTestForm1Id").disabled = false;
}

function resetTestForm1() {
    document.getElementById("testForm1").reset();
}

var myPopUpWindow;
function openPopUpWindow() {
    myPopUpWindow = window.open("https://www.google.com", "myPopUpWindow", "width=640, height=480");
    //myPopUpWindow.document.write("<p>This is 'myPopUpWindow'</p>")
}
function closePopUpWindow() {
    myPopUpWindow.close();
}

function processForm2() {
	if (document.getElementById("rbtn1Id").checked == true) {
		alert(document.getElementById("rbtn1Id").value);
		return false;
	} else if (document.getElementById("rbtn2Id").checked == true) {
		alert(document.getElementById("rbtn2Id").value);
		return false;
	} else {
		alert("no radio button clicked");
		return false;
	}
}

function Person (firstName, lastName) {  
    this._firstName = firstName;
    this._lastName = lastName;
    this.toString = function() {
        return this._firstName + ' ' + this._lastName ;
    };
}

function processForm3() {
    var firstName = document.forms["testForm3"]["txtFirstNameId"].value;
	var lastName = document.forms["testForm3"]["txtLastNameId"].value;
	var person = new Person(firstName, lastName);
	alert(person.toString());
}

function resetTestForm3() {
    document.getElementById("testForm3").reset();
}
