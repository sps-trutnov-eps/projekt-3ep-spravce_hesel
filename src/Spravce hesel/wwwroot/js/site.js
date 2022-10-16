function PrepnoutBarevnyMotiv() {
    let cookies = document.cookie;

    if (cookies.length == 0)
        document.cookie = "TmavyMotiv=true; path=/; expires=Fri, 31 Dec 9999 23:59:59 GMT";
    else {
        document.cookie = "TmavyMotiv=; path=/; max-age=0";
    }

    window.location.reload();
    }

function zobrazitHeslo(id) {
    $.ajax({
        type: "GET",
        url: "/Hesla/DetailHesla/" + id,
        dataType: "JSON",
        data: JSON.stringify(id),
        contentType: "application/json; charset=utf-8",
        
        success: (res) => {
            document.getElementById("detaily_pozadi").className = "";
            document.getElementById("detaily").className = "";

            document.getElementById("sluzba").innerHTML = res.value.sluzba;
            document.getElementById("jmeno").innerHTML = res.value.jmeno;
            document.getElementById("heslo").innerHTML = res.value.sifra;
        }
    });

    // fetch("/Hesla/DetailHesla/" + id, { method: "GET", dataType: "JSON", url: "/Hesla/DetailHesla/" + id, }).then(data => data.json()).then(data => console.log(data));
}

function skrytHeslo() {
    document.getElementById("detaily").className = "skryty";
    document.getElementById("detaily_pozadi").className = "skryty";
}