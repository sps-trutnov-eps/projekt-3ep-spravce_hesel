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
    fetch("/Hesla/DetailHesla/" + id, {
        method: "GET"
        }).then(data => data.json()).then(data => console.log(data));
}