// Barevný motiv
function PrepnoutBarevnyMotiv() {
    if (!document.cookie.includes("TmavyMotiv"))
        document.cookie = "TmavyMotiv=true; path=/; expires=Fri, 31 Dec 9999 23:59:59 GMT";
    else
        document.cookie = "TmavyMotiv=; path=/; max-age=0";

    window.location.reload();
    }

// Vyžádání hesla
function vyzadatHeslo(id) {
    $.ajax({
        type: "GET",
        url: "/Hesla/DetailHesla/" + id,
        dataType: "JSON",
        data: JSON.stringify(id),
        contentType: "application/json; charset=utf-8",
        
        success: (res) => {
            zobrazitPOPup("detaily");
            if (res.value.sluzba == null || res.value.sluzba.trim() == "")
                document.getElementById("sluzba").innerHTML = decodeURIComponent("<i>Neznámá služba</i>");
            else
                document.getElementById("sluzba").innerHTML = decodeURIComponent(res.value.sluzba);

            if (res.value.jmeno == null || res.value.jmeno.trim() == "")
                document.getElementById("jmeno").innerHTML = decodeURIComponent("<i>Neznámé jméno</i>");
            else
                document.getElementById("jmeno").innerHTML = decodeURIComponent(res.value.jmeno);

            document.getElementById("heslo").innerHTML = decodeURIComponent(res.value.sifra);
        }
    });
}

// Vyžádání potvrzení odstranění
function vyzadatOdstraneni(id) {
    document.getElementById("odstraneni").action = "/Hesla/Odstranit/" + id;
    zobrazitPOPup("odstranit");
}

// Vyžádání potvrzení
let zmenyVPotvrzeni = false;
function vyzadatPotvrzeni(id, rozhodnuti) {
    if (rozhodnuti == true) {
        $.ajax({
            type: "POST",
            url: "/Hesla/PotvrditSdileni/" + id,
            dataType: "JSON",
            data: JSON.stringify(id),
            contentType: "application/json; charset=utf-8",

            success: (res) => {
                document.getElementsByClassName("oznameni " + id)[0].innerHTML = "<p>Rozhodnutí uloženo</p>";
            }
        });
    }
    else {
        $.ajax({
            type: "POST",
            url: "/Hesla/ZrusitSdileni/" + id,
            dataType: "JSON",
            data: JSON.stringify(id),
            contentType: "application/json; charset=utf-8",

            success: (res) => {
                document.getElementsByClassName("oznameni " + id)[0].innerHTML = "<p>Rozhodnutí uloženo</p>";
            }
        });
    }

    zmenyVPotvrzeni = true;
}

// pop-up
function zobrazitPOPup(id) {
    skrytPOPUp();
    document.getElementsByClassName("POPup")[id].className = "POPup";
}

function skrytPOPUp() {
    if (zmenyVPotvrzeni == true)
        window.location.reload();
    else {
        for (let div of document.getElementsByClassName("POPup")) {
            div.className = "POPup skryty";
        };
    }
}

$('div.POPup').click(function (e) {
    if (e.target == this) {
        skrytPOPUp()
    }
});

// Udělení souhlasu
function udeleniSouhlasu() {
    document.cookie = "Souhlas=true; path=/; expires=Fri, 31 Dec 9999 23:59:59 GMT";
    window.location.reload();
}

