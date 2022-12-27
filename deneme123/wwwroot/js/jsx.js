var $doc = $(document),
    $body = $('body'),
    y = $doc.scrollTop(),
    color1 = 0,
    color2 = 0;

$doc.on('scroll', function () {
    y = $doc.scrollTop();
    color1 = y / 30;
    color2 = color1 + 180;
    $body.css('background', 'hsla(' + color1 + ', 30%, 45%, 1)');
    $body.find('h1').css('color', 'hsla(' + color2 + ', 4%, 40%, 1)');
});
