// Vyžádání hesla
function vyzadatHeslo(id, norm = true) {
    let url = "DetailSdilenehoHesla";
    if (norm == true)
        url = "DetailHesla";

    $.ajax({
        type: "GET",
        url: "/Hesla/" + url + "/" + id,
        dataType: "JSON",
        data: JSON.stringify(id),
        contentType: "application/json; charset=utf-8",
        
        success: (res) => {
            zobrazitPOPup("detaily");
            if (res.value.sluzba == null || res.value.sluzba.trim() == "")
                document.getElementById("sluzba").innerHTML = "<i>Neznámá služba</i>";
            else
                document.getElementById("sluzba").innerHTML = res.value.desifrovanaSluzba;

            if (res.value.jmeno == null || res.value.jmeno.trim() == "")
                document.getElementById("jmeno").innerHTML = "<i>Neznámé jméno</i>";
            else
                document.getElementById("jmeno").innerHTML = res.value.desifrovaneJmeno;

            document.getElementById("heslo").innerHTML = res.value.desifrovaneHeslo;
            document.getElementById("sdilenoOd").innerHTML = "";

            if (norm == true) {
                document.getElementById("detaily").firstElementChild.className = "";
            }
            else {
                document.getElementById("detaily").firstElementChild.className = "sdileneheslo";
                document.getElementById("sdilenoOd").innerHTML = "<i>Sdíleno od " + res.value.zakladatelJmeno + "</i>";
            }

        }
    });
}

// Vyžádání potvrzení odstranění
function vyzadatOdstraneni(id, norm = true) {
    let url = "OdstranitSdilene";
    if (norm == true) {
        url = "Odstranit";
    }
    document.getElementById("odstraneni").action = "/Hesla/" + url + "/" + id;
    zobrazitPOPup("odstranit");
}

// Vyžádání potvrzení
let zmenyVPotvrzeni = false;
function vyzadatPotvrzeni(id, rozhodnuti = true) {
    let url = "ZrusitSdileni";
    if (rozhodnuti == true)
        url = "PotvrditSdileni";

    $.ajax({
        type: "POST",
        url: "/Hesla/" + url + "/" + id,
        dataType: "JSON",
        data: JSON.stringify(id),
        contentType: "application/json; charset=utf-8",

        success: (res) => {
            document.getElementsByClassName("oznameni " + id)[0].innerHTML = "<p>Rozhodnutí uloženo</p>";
            zmenyVPotvrzeni = true;
        }
    });
}

// pop-up
function zobrazitPOPup(id) {
    skrytPOPUp();
    document.getElementsByClassName("POPup")[id].className = "POPup";
}

function skrytPOPUp() {
    if (zmenyVPotvrzeni == true)
        window.location.href = "/Hesla/Zobrazeni";
    else {
        for (let div of document.getElementsByClassName("POPup")) {
            div.className = "POPup skryty";
        };
    }
}

// Udělení souhlasu
$(".udeleni_souhlasu").click(function () {
    document.cookie = "Souhlas=true; path=/; expires=Fri, 31 Dec 9999 23:59:59 GMT";
    window.location.reload();
});

// Načítání
$("form").submit(function (e) {
    if (e.target == this) {
        document.querySelector("#nacitani").className = "";
    }
});

// Skrytí popupu
$("div.POPup").click(function (e) {
    if (e.target == this) {
        skrytPOPUp();
    }
});

// Tlačítko přidání
$(".tlacitko_pridat").click(() => {
    window.location.href = '/Hesla/Pridat';
});

// Tlačítko oznámení
$(".tlacitko_oznameni").click(() => {
    zobrazitPOPup('oznameni');
});

// Tlačítko barevného motivu
$(".barevny_motiv").click(() => {
    if (!document.cookie.includes("TmavyMotiv"))
        document.cookie = "TmavyMotiv=true; path=/; expires=Fri, 31 Dec 9999 23:59:59 GMT";
    else
        document.cookie = "TmavyMotiv=; path=/; max-age=0";

    window.location.reload();
});

