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
            document.getElementById("sluzba").innerHTML = res.value.sluzba;
            document.getElementById("jmeno").innerHTML = res.value.jmeno;
            document.getElementById("heslo").innerHTML = res.value.sifra;
        }
    });
}

// Vyžádání potvrzení odstranìní
function vyzadatOdstraneni(id) {
    document.getElementById("odstraneni").action = "/Hesla/Odstranit/" + id;
    zobrazitPOPup("odstranit");
}

// pop-up
function zobrazitPOPup(id) {
    skrytPOPUp();
    document.getElementsByClassName("POPup")[id].className = "POPup";
}

function skrytPOPUp() {
    for (let div of document.getElementsByClassName("POPup")) {
        div.className = "POPup skryty";
    };
}

$('div.POPup').click(function (e) {
    if (e.target == this) {
        skrytPOPUp()
    }
});
