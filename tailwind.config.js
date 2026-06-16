/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./Components/**/*.razor",
    "./Components/**/*.razor.cs",
    "./Pages/**/*.razor",
    "./Shared/**/*.razor",
    "./wwwroot/**/*.html",
    "./App.razor"
  ],
  theme: {
    extend: {
      fontFamily: {
        heading: ["Outfit", "sans-serif"],
        body: ["Plus Jakarta Sans", "sans-serif"],
        sutonnymj: ["SutonnyMJ", "sans-serif"]
      }
    }
  },
  plugins: []
}
