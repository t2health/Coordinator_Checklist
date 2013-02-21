/**
 * 
 * 
 * All actions here get sent to HTML5FormPlugin.execute and pass the action name.
 * 
 * @return Instance of HTML5FormPlugin
 */
var HTML5FormPlugin = {
logFormData: function(types, success, fail) {
    return cordova.exec(success, fail, "HTML5FormPlugin", "logFormData", types);
    }
};

function logFormResults(formData){
    HTML5FormPlugin.logFormData(
                          [formData],
                          function(result){
                          console.log('Success : ' + result);
                          },
                          function(result){
                          console.log('Error : ' + result);
                          }
                          );
}