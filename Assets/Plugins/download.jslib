mergeInto(LibraryManager.library, {
  DownloadFile : async function(array, size, fileNamePtr) {
    const fileName = UTF8ToString(fileNamePtr);
    const bytes = new Uint8Array(size);
    for (let i = 0; i < size; i++) {
        bytes[i] = HEAPU8[array + i];
    }
    const blob = new Blob([bytes]);
    const link = document.createElement('a');
    link.href = window.URL.createObjectURL(blob);
    link.download = fileName;
    const event = document.createEvent("MouseEvents");
    event.initMouseEvent("click");
    link.dispatchEvent(event);
    window.URL.revokeObjectURL(link.href);
  }
});
