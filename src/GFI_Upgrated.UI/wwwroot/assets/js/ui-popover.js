"use strict";

(function () {
  const providers = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
  if (!providers.length) {
    return;
  }

  providers.forEach(function (provider) {
    try {
      const containerClass = provider.getAttribute('data-bs-container-class');
      const options = containerClass ? { container: `.${containerClass}` } : {};
      new bootstrap.Popover(provider, options);
    } catch (error) {
      console.error('Popover initialization failed.', error);
    }
  });
})();
