
function decompress(byteArray) {
    const cs = new DecompressionStream("gzip");
    const writer = cs.writable.getWriter();
    writer.write(byteArray);
    writer.close();
    return new Response(cs.readable).arrayBuffer().then(function (arrayBuffer) {
        return new TextDecoder().decode(arrayBuffer);
    });
}


// Source - https://stackoverflow.com/a/21797381
// Posted by Goran.it, modified by community. See post 'Timeline' for change history
// Retrieved 2026-05-31, License - CC BY-SA 4.0

function base64ToArrayBuffer(base64) {
    var binaryString = atob(base64);
    var bytes = new Uint8Array(binaryString.length);
    for (var i = 0; i < binaryString.length; i++) {
        bytes[i] = binaryString.charCodeAt(i);
    }
    return bytes.buffer;
}

const input=document.getElementById("input");

document.getElementById("btn").addEventListener("click", async () => {
    const arrayBuffer = base64ToArrayBuffer(input.value);
    const decompressed = await decompress(arrayBuffer);
    input.value = decompressed;
});
