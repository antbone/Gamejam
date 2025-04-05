"use strict";var __importDefault=this&&this.__importDefault||function(t){return t&&t.__esModule?t:{default:t}};Object.defineProperty(exports,"__esModule",{value:!0}),exports.IOUtils=void 0;const cli_color_1=__importDefault(require("cli-color")),fs_1=__importDefault(require("fs")),path_1=__importDefault(require("path")),StrUtils_1=require("./StrUtils"),enc_latin1_1=__importDefault(require("crypto-js/enc-latin1")),enc_hex_1=__importDefault(require("crypto-js/enc-hex")),md5_1=__importDefault(require("crypto-js/md5")),LineBreak_1=require("./LineBreak");class IOUtils{static makeDir(t){return!!fs_1.default.existsSync(t)||(this.makeDir(path_1.default.dirname(t))?(fs_1.default.mkdirSync(t),!0):void 0)}static findFile(r,a=[],l=[]){const t=fs_1.default.readdirSync(r);return t.forEach(t=>{var e=path_1.default.join(r,t);const i=fs_1.default.statSync(e);i.isDirectory()?this.findFile(path_1.default.join(r,t),a,l):(t=path_1.default.extname(e),a.length&&-1==a.indexOf(t)||l.push(e))}),l}static findFileByCondition(e,i,r=[]){if(fs_1.default.existsSync(e)){let t=fs_1.default.readdirSync(e);t.forEach(t=>{t=path_1.default.join(e,t);fs_1.default.statSync(t).isDirectory()?this.findFileByCondition(t,i,r):i(t)&&r.push(t)})}}static findDirectory(r,a=[]){const t=fs_1.default.readdirSync(r);return t.forEach(t=>{var e=path_1.default.join(r,t);const i=fs_1.default.statSync(e);i.isDirectory()&&(a.push(e),this.findDirectory(path_1.default.join(r,t),a))}),a}static findDirectoryByCondition(r,a,l=[]){const t=fs_1.default.readdirSync(r);return t.forEach(t=>{var e=path_1.default.join(r,t);const i=fs_1.default.statSync(e);i.isDirectory()&&(a(e)&&l.push(e),this.findDirectory(path_1.default.join(r,t),l))}),l}static deleteFile(t){fs_1.default.existsSync(t)&&fs_1.default.unlinkSync(t)}static deleteFolderFileByCondition(e,i){if(fs_1.default.existsSync(e)){let t=fs_1.default.readdirSync(e);t.forEach(t=>{t=path_1.default.join(e,t);fs_1.default.statSync(t).isDirectory()?this.deleteFolderFileByCondition(t,i):i(t)&&fs_1.default.unlinkSync(t)})}}static deleteFolderFile(e,t=!0){let i=[];fs_1.default.existsSync(e)&&((i=fs_1.default.readdirSync(e)).forEach(t=>{t=e+"/"+t;fs_1.default.statSync(t).isDirectory()?this.deleteFolderFile(t):fs_1.default.unlinkSync(t)}),t&&fs_1.default.rmdirSync(e))}static fileOrFolderIsExsit(t){try{return fs_1.default.accessSync(t),!0}catch(t){return!1}}static getFileMD5(t){if(this.fileOrFolderIsExsit(t))return t=fs_1.default.readFileSync(t,{encoding:"latin1"}),(0,md5_1.default)(enc_latin1_1.default.parse(t)).toString(enc_hex_1.default)}static writeTextFile(writePath,content,lineBreak,succeedLog,failedLog){if(!content)return console.log(cli_color_1.default.yellow("Cannot write null. ->"+writePath));if(null!=lineBreak)switch(lineBreak){case LineBreak_1.LineBreak.CRLF:{let pwd=StrUtils_1.StrUtils.genPassword(8);content=content.replace(/\r\n/g,pwd),content=content.replace(/\n/g,`\r
`),content=content.replace(/\r/g,"");var reg="/"+pwd+"/g";content=content.replace(eval(reg),`\r
`);break}case LineBreak_1.LineBreak.LF:content=content.replace(/\r/g,"")}try{fs_1.default.writeFileSync(writePath,content,{encoding:"utf-8"}),succeedLog&&console.log(cli_color_1.default.green(StrUtils_1.StrUtils.format(succeedLog,writePath)))}catch(error){if(failedLog)console.log(cli_color_1.default.red(StrUtils_1.StrUtils.format(failedLog,writePath,error||"")));else if(null==failedLog)throw error}}static copy(a,l){if(0==fs_1.default.existsSync(a))return!1;if(this.makeDir(l),fs_1.default.statSync(a).isDirectory()){var t=fs_1.default.readdirSync(a);let r=this;t.forEach(function(t){var e=path_1.default.join(a,t),i=fs_1.default.statSync(e);i.isFile()?fs_1.default.copyFileSync(e,path_1.default.join(l,t)):i.isDirectory()&&r.copy(e,path_1.default.join(l,t))})}else{t=path_1.default.basename(a);fs_1.default.copyFileSync(a,path_1.default.join(l,t))}}}exports.IOUtils=IOUtils;