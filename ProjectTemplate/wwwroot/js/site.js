// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//-----------------------------------------------------------------------------------------------START OF ITEM VIEW JS------------------------------------------------------------------------------
const thumbs = document.querySelectorAll('.thumb');

const mainImage = document.getElementById('mainImage');

thumbs.forEach(img => {

    img.addEventListener('click',()=>{

        thumbs.forEach(t =>
            t.classList.remove('active'));

        img.classList.add('active');

        mainImage.src = img.src;

    });

});

document
.querySelectorAll('.size-btn')
.forEach(btn=>{

    btn.addEventListener('click',()=>{
        document
        .querySelectorAll('.size-btn')
        .forEach(b =>
            b.classList.remove('active'));
        btn.classList.add('active');
    });
});

const qty = document.getElementById('qty');

document.getElementById('plus').onclick = ()=> {
    qty.value =
    Number(qty.value)+1;
};

document.getElementById('minus').onclick = ()=> {
    if(qty.value > 1){
        qty.value =
        Number(qty.value)-1;
    }
};

document
.querySelector('.cart-btn')
.addEventListener('click',()=> {

    const size =
    document.querySelector('.size-btn.active');

    if(!size){

        alert(
        'Please select a size.');

        return;
    }

    alert(
      'Added to Cart Successfully!'
    );

});

const dialog = document.getElementById('sizeDialog');
dialog.addEventListener('click',(e)=>{
    const rect =
    dialog.getBoundingClientRect();
    const inside =
        rect.top <= e.clientY &&
        e.clientY <= rect.bottom &&
        rect.left <= e.clientX &&
        e.clientX <= rect.right;
    if(!inside){
        dialog.close();
    }
});
//------------------------------------------------------------------------------------------END OF ITEM VIEW JS------------------------------------------------------------------------------