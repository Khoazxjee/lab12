$.extend($.validator.messages, {
    required: "Vui lòng nhập thông tin bắt buộc.",
    number: "Vui lòng nhập một số hợp lệ.",
    date: "Vui lòng nhập ngày hợp lệ.",
    email: "Email không đúng định dạng."
});
// Date Range dùng ngày ISO từ input; các trường số dùng bộ kiểm tra gốc.
const numericRange = $.validator.methods.range;
$.validator.methods.range = function (value, element, parameters) {
    if (element.type === "date") {
        return this.optional(element) || ((!element.min || value >= element.min) && (!element.max || value <= element.max));
    }
    return numericRange.call(this, value, element, parameters);
};
