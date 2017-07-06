/**************************************************

通用数据类型检测工具 .js

作者：江龙
初稿：2014-04-10
修订：2014-04-15 v1.0  完成
      2014-04-18 V1.0.1a  增加$t.Val(),$t.ToNumberX() 等格式
　　　加入日期定义，并可以通过
　　　$t.SetDateRange(obj,name,rel)
　　　来实现(obj表示菜单项,name:表示需要设置的defaultRanges名字
　　　obj, 一般用this
　　　name, 默认名，若没有设置或者为空，则取当前对象的text值
　　　rel: datarange捆绑的input id
　　　　　　若没有自动找到控件内的input
********************************************/

        
$t={ 
version:"1.0.1a",
author:"江龙",
typeSplit:/[\s，；,;\|]/,
roles:
{
"int":      {bit:32,type:'i',name:"整形",regex:/^\-?\d{1,10}$/},
"int32":    {bit:32,type:'i',name:"整形",regex:/^\-?\d{1,10}$/},
"integer":  {bit:32,type:'i',name:"整形",regex:/^\-?\d{1,10}$/},
"bigint":   {bit:64,type:'i',name:"长整形",regex:/^\-?\d{1,19}$/},
"int64":    {bit:64,type:'i',name:"长整形",regex:/^\-?\d{1,19}$/},
"long":     {bit:64, type:'i',name:"长整形",regex:/^\-?\d{1,19}$/},
"int16":    {bit:16,  type:'i',name:"短整形",regex:/^\-?\d{1,5}$/},
"smallint": {bit:16, type:'i',name:"短整形",regex:/^\-?\d{1,5}$/},
"short":    {bit:16,name:"短整形",type:'i',regex:/^\-?\d{1,5}$/},
"byte":     {bit:8,name:"字节型",type:'i',regex:/^\-?\d{1,3}$/},
"tinyint":  {bit:8,name:"短整形",type:'i',regex:/^\-?\d{1,3}$/},
"bit":      {name:"位",type:'b',regex:/^[01]{1}$/},
"bool":     {name:"真假型",type:"bool"},
"boolean":  {name:"真假型",type:"bool"},
"number":   {name:"数值型",min: -Math.pow(10.0,38)+1,max: Math.pow(10.0,38)-1,type:'n',scale:0,regex:/^\-?\d*(\.\d*)?$/},
"decimal":  {name:"数值型",min: -Math.pow(10.0,38)+1,max: Math.pow(10.0,38)-1,type:'n',scale:0},
"money":    {min:-922337203685477.5808,len:18,  type:'n',name:"货币型",  max:922337203685477.5807,scale:4},
"smallmoney": { min: -214748.3648, len: 8, max: 214748.3647, name: "短货币型", type: 'n', scale: 4 },
"float":     {min:-1.79E+308,name:"浮点数", max:1.79E+308, type:'f'},
"real":      {min:-3.40E+38,max:3.40E+38 ,name:"实型数",type:'f'},
"time":      {min:'00:00:00',df:1, max:'23:59:59', type:'t',format:'hh:mm:ss'}, 
"time1":     {min:'00:00:00',df:1, max:'23:59:59', type:'t',format:'hh:mm:ss'}, 
"time2":     {min:'00:00:00',df:2, max:'23:59:59', type:'t',format:'hh:mm:ss'}, 
"time4":     {min:'00时00分00秒',df:4, max:'23时59分59秒', type:'t',format:'hh时mm分ss秒'}, 
"time3":     {min:'000000',df:3, max:'235959', type:'t',format:'hhmmss'}, 

"date"          :{min:'1753-01-01',df:1,max:'9999-12-31',name:"日期型",type:'d',format:'yyyy-MM-dd'},
"smalldate"     :{min:'1900-01-01',df:1,name:"短日期型",  max:'2079-06-06',type:'d',  maxlength:10,format:'yyyy-MM-dd'},
"datetime"      :{min:'1753-01-01 00:00:00',df:1, time:' 00:00:00', max:'9999-12-31 23:59:59',name:"日期时间型",type:'dt',format:'yyyy-MM-dd hh:mm:ss'},
"smalldatetime" :{min:'1900-01-01 00:00:00',df:1,type:'dt', time:' 00:00:00',name:"短日期时间型",max:'2079-06-06 23:59:59',format:'yyyy-MM-dd hh:mm:ss'},
"date1"         :{min:'1753-01-01',df:1,max:'9999-12-31',name:"日期型",type:'d',format:'yyyy-MM-dd'},
"smalldate1"    :{min:'1900-01-01',df:1,name:"短日期型",  max:'2079-06-06',type:'d',  maxlength:10,format:'yyyy-MM-dd'},
"datetime1"     :{min:'1753-01-01 00:00:00', df:1, time:' 00:00:00',max:'9999-12-31 23:59:59',name:"日期时间型",type:'dt',format:'yyyy-MM-dd hh:mm:ss'},
"smalldatetime1":{min:'1900-01-01 00:00:00',df:1,time:' 00:00:00',type:'dt', name:"短日期时间型",max:'2079-06-06 23:59:59',format:'yyyy-MM-dd hh:mm:ss'},
"date2"         :{min:'17530101',max:'99991231',df:2,name:"日期型",type:'d',maxlength:8,format:'yyyy/MM/dd'},
"smalldate2"    :{min:'19000101',name:"短日期型",df:2,  max:'20790606',type:'d',  maxlength:8,format:'yyyy/MM/dd'},
"datetime2"     :{min:'17530101000000',df:2,time:' 00:00:00',  max:'99991231235959',name:"日期时间型",type:'dt',maxlength:14,format:'yyyy/MM/dd hh:mm:ss'},
"smalldatetime2":{min:'19000101000000',df:2,time:' 00:00:00',type:'dt', name:"短日期时间型",max:'20790606235959',maxlength:14,format:'yyyy/MM/dd hh:mm:ss'},

"date3"         :{min:'17530101',df:3,max:'99991231',name:"日期型",type:'d',maxlength:8,format:'yyyyMMdd'},
"smalldate3"    :{min:'19000101',df:3,name:"短日期型",  max:'20790606',type:'d',  maxlength:8,format:'yyyyMMdd'},
"datetime3"     :{min:'17530101000000',df:3, time:'000000', max:'99991231235959',name:"日期时间型",type:'dt',maxlength:14,format:'yyyyMMddhhmmss'},
"smalldatetime3":{min:'19000101000000',df:3,time:'000000',type:'dt', name:"短日期时间型",max:'20790606235959',maxlength:14,format:'yyyyMMddhhmmss'},

"date4"         :{min:'1753年01月01日',df:4,max:'9999年12月31日',name:"日期型",type:'d',maxlength:8,format:'yyyy年MM月dd日'},
"smalldate4"    :{min:'1900年01月01日',df:4,name:"短日期型",  max:'2079年06月06日',type:'d',  maxlength:8,format:'yyyy年MM月dd日'},
"datetime4"     :{min:'1753年01月01日00时00分00秒',df:4, time:'00时00分00秒', max:'9999年12月31日23时59分59秒',name:"日期时间型",type:'dt',maxlength:14,format:'yyyy年MM月dd日hh时mm分ss秒'},
"smalldatetime4":{min:'1900年01月01日00时00分00秒',df:4,time:'00时00分00秒',type:'dt', name:"短日期时间型",max:'2079年06月06日23时59分59秒',maxlength:14,format:'yyyy年MM月dd日hh时mm分ss秒'},

"char"      :{ minlength:0, maxlength:8000, name:"字节型" ,type:'s',unicode:false},
"varchar"   :{ minlength:0,maxlength:8000,name:"可变字符",type:'s',unicode:false},
"string"    :{ minlength:0,maxlength:2147483647,name:"可变字符",type:'s',unicode:false},
"text"      :{ minlength:0,maxlenth:2147483647,name:"文本型",type:'s',unicode:false},
"nchar"     :{minlength:0,maxlength:4000,name:"文本型",unicode:true,type:'s'},
"nvarchar"  :{name:"宽字符串",minlength:0,maxlength:4000,unicode:true,type:'s'},
"ntext"     :{name:"长文本",minlength:0,maxlength:1073741823,unicode:true,type:'s'},
"nstring"   :{name:"长文本",minlength:0,maxlength:1073741823,unicode:true,type:'s'},
"ip"        :{name:"ＩＰ地址",regex:/([1-9]|[1-9]\d|1\d{2}|2[0-4]\d|25[0-5])(\.(\d|[1-9]\d|1\d{2}|2[0-4]\d|25[0-5])){3}$/},
"url"       :{ name:"网址",regex:/^(http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?$/},
"tel"       :{ name:"电话", regex:/^((0\d{2,3})-)?(\d{7,8})(-(\d{3,}))?$/ },
"mobile"    :{name:"手机", regex:/^1(\d{10})$/ },
"phone"     :{ name:"电话",regex:/^((\d{11})|^((\d{7,8})|(\d{4}|\d{3})-(\d{7,8})|(\d{4}|\d{3})-(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1})|(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1}))$)/},
"email"     :{name:"Email",regex:/^(\w)+((\.|\-)\w+)*@(\w)+((\.\w{2,3}){1,3})$/},
"mac"       :{name:"MAC地址",regex:/^[A-F\d]{2}:[A-F\d]{2}:[A-F\d]{2}:[A-F\d]{2}:[A-F\d]{2}:[A-F\d]{2}$/},
"id15"      :{name:"15位身份证号", check:function(v){ return $t.IsID15(v);},msg:'不是15位身份证号'},
"id18"      :{name:"18位身份证号", check:function(v){ return $t.IsID18(v);},msg:'不是18位身份证号'},
"id"        :{name:"身份证号",check:function(v){ return $t.IsID(v);},msg:'不是身份证号'},
"bankcard"  :{name:"银行卡",regex:/^(998801|998802|622525|622526|435744|435745|483536|528020|526855|622156|622155|356869|531659|622157|627066|627067|627068|627069)\d{10}$/},
"digi"      :{regex:/^(\d){1,}$/,name:"数字"},
"webcolor"  :{regex:/^#?([a-f]|[A-F]|[0-9]){3}(([a-f]|[A-F]|[0-9]){3})?$/,name:""},
"lower"     :{regex:/^[a-z]{1,}$/,name:"小写字母"},
"upper"     :{regex:/^[A-Z]{1,}$/,name:"大写字母"},
"ldu"       :{regex:/^[a-zA-Z0-9]{1,}$/,name:"英文与数字、下画线"},
"english"   :{regex:/^[a-zA-Z]{1,}$/,name:"英文字母"},
"hz": { regex: /^[\u4e00-\u9fa5]{1,}$/, name: "全汉字" },
"requird"   :{name:"非空",check:function(v){ return typeof(v)!="undefined" && v!=null && $.trim(v)!="";},msg:'不能为空值' }
},
IsTime      :function(s,t){return this.ToTime(s,t)!=null;},
IsDate      :function(s,t){return this.ToDate(s,t)!=null;},
IsDateTime  :function(s,t){return this.ToDateTime(s,t)!=null;},

IsTime1      :function(s){return this.ToTime(s,1)!=null;},
IsDate1      :function(s){return this.ToDate(s,1)!=null;},
IsDateTime1  :function(s){return this.ToDateTime(s,1)!=null;},


IsTime2      :function(s){return this.ToTime(s,2)!=null;},
IsDate2      :function(s){return this.ToDate(s,2)!=null;},
IsDateTime2  :function(s){return this.ToDateTime(s,2)!=null;},


IsTime3      :function(s){return this.ToTime(s,3)!=null;},
IsDate3      :function(s){return this.ToDate(s,3)!=null;},
IsDateTime3  :function(s){return this.ToDateTime(s,3)!=null;},

IsTime4      :function(s){return this.ToTime(s,4)!=null;},
IsDate4      :function(s){return this.ToDate(s,4)!=null;},
IsDateTime4  :function(s){return this.ToDateTime(s,4)!=null;},


IsFloat     :function(s){return this.ToFloat(s)!=null;},
IsInt       :function(s){return this.ToInt(s)!=null;},
IsNull      :function(s){return (s==null || typeof(s)=="undefined" || $.trim(s)=="");},
IsNumber    :function(s){return this.IsTypeRegex(s,"number");},
IsDigi      :function(s){return this.IsTypeRegex(s,"digi");},
IsLower     :function(s){return this.IsTypeRegex(s,"lower");},
IsUpper     :function(s){return this.IsTypeRegex(s,"upper");},
IsLetter    :function(s){return this.IsTypeRegex(s,"letter");},
IsLetterDigiUnderline:function(s){return this.IsLdu(s);},
IsLdu       :function(s){return this.IsTypeRegex(s,"ldu");},
IsName      :function(s){return this.IsTypeRegex(s,"name");},
IsBankCardNo:function(s){return this.IsTypeRegex(s,"bankcardno");},
IsEmail     :function(s){return this.IsTypeRegex(s,"email");},
IsID:function(s){  return this.ToID(s)!=null; },
IsID15:function(s){  return this.ToID15(s)!=null; },
IsID18:function(s){  return this.ToID18(s)!=null; },
ToID        :function(s)
{
   var id=this.ToID18(s);
   if(id==null)
       id=this.ToID15(s);
   return id;
},
ToUpperRMB:function (currencyDigits) {
        // Constants:  
        var MAXIMUM_NUMBER = 99999999999.99;
        // Predefine the radix characters and currency symbols for output:  
        var CN_ZERO = "零";
        var CN_ONE = "壹";
        var CN_TWO = "贰";
        var CN_THREE = "叁";
        var CN_FOUR = "肆";
        var CN_FIVE = "伍";
        var CN_SIX = "陆";
        var CN_SEVEN = "柒";
        var CN_EIGHT = "捌";
        var CN_NINE = "玖";
        var CN_TEN = "拾";
        var CN_HUNDRED = "佰";
        var CN_THOUSAND = "仟";
        var CN_TEN_THOUSAND = "万";
        var CN_HUNDRED_MILLION = "亿";
        var CN_SYMBOL = "";// "人民币";
        var CN_DOLLAR = "元";
        var CN_TEN_CENT = "角";
        var CN_CENT = "分";
        var CN_INTEGER = "整";

        // Variables:  
        var integral; // Represent integral part of digit number.  
        var decimal; // Represent decimal part of digit number.  
        var outputCharacters; // The output result.  
        var parts;
        var digits, radices, bigRadices, decimals;
        var zeroCount;
        var i, p, d;
        var quotient, modulus;

        // Validate input string:  
        currencyDigits = currencyDigits +"";//.toString();
        if (currencyDigits == "") {
            //alert("Empty input!");
            return "";
        }
        if (currencyDigits.match(/[^,.\d]/) != null) {
            // alert("Invalid characters in the input string!");
            return "(无效数值)";
        }
        if ((currencyDigits).match(/^((\d{1,3}(,\d{3})*(.((\d{3},)*\d{1,3}))?)|(\d+(.\d+)?))$/) == null) {
            //alert("Illegal format of digit number!");
            return "(非法数值)";
        }

        // Normalize the format of input digits:  
        currencyDigits = currencyDigits.replace(/,/g, ""); // Remove comma delimiters.  
        currencyDigits = currencyDigits.replace(/^0+/, ""); // Trim zeros at the beginning.  
        // Assert the number is not greater than the maximum number.  
        if (Number(currencyDigits) > MAXIMUM_NUMBER) {
            //alert("数据太大!");
            return "(数据太大)";
        }

        // Process the coversion from currency digits to characters:  
        // Separate integral and decimal parts before processing coversion:  
        parts = currencyDigits.split(".");
        if (parts.length > 1) {
            integral = parts[0];
            decimal = parts[1];
            // Cut down redundant decimal digits that are after the second.  
            decimal = decimal.substr(0, 2);
        }
        else {
            integral = parts[0];
            decimal = "";
        }
        // Prepare the characters corresponding to the digits:  
        digits = new Array(CN_ZERO, CN_ONE, CN_TWO, CN_THREE, CN_FOUR, CN_FIVE, CN_SIX, CN_SEVEN, CN_EIGHT, CN_NINE);
        radices = new Array("", CN_TEN, CN_HUNDRED, CN_THOUSAND);
        bigRadices = new Array("", CN_TEN_THOUSAND, CN_HUNDRED_MILLION);
        decimals = new Array(CN_TEN_CENT, CN_CENT);
        // Start processing:  
        outputCharacters = "";
        // Process integral part if it is larger than 0:  
        if (Number(integral) > 0) {
            zeroCount = 0;
            for (i = 0; i < integral.length; i++) {
                p = integral.length - i - 1;
                d = integral.substr(i, 1);
                quotient = p / 4;
                modulus = p % 4;
                if (d == "0") {
                    zeroCount++;
                }
                else {
                    if (zeroCount > 0) {
                        outputCharacters += digits[0];
                    }
                    zeroCount = 0;
                    outputCharacters += digits[Number(d)] + radices[modulus];
                }
                if (modulus == 0 && zeroCount < 4) {
                    outputCharacters += bigRadices[quotient];
                }
            }
            outputCharacters += CN_DOLLAR;
        }
        // Process decimal part if there is:  
        if (decimal != "")
        {
            
            var ts = "";
            for (i = 0; i < decimal.length; i++)
            {
                d = decimal.substr(i, 1);
                if (d != "0") {
                    ts += digits[Number(d)] + decimals[i];
                }
            }
            if (outputCharacters != "" && ts!="" && ts.indexOf("角") == -1)
                ts = "零" + ts;
            outputCharacters += ts;
        }
        // Confirm and return the final output string:  
        if (outputCharacters == "") {
            outputCharacters = CN_ZERO + CN_DOLLAR;
        }
        if (decimal == "") {
            outputCharacters += CN_INTEGER;
        }
        outputCharacters = CN_SYMBOL + outputCharacters;
        var tn=outputCharacters.length;
        var endc=outputCharacters.substring(tn-1,tn)
        if (endc != "整" && endc == "元")
            outputCharacters += "整";
        return outputCharacters;
    },
ToID15      :function(v)
{
var reg=/^[1-9]\d{5}(\d{2})(\d{2})(\d{2})\d{3}$/;
var r=v.match(reg);
if(r==null) return false;
var d=this.ToDate("19"+r[1]+r[2]+r[3],3);
if(d!=null)
   return v;
else
   return null;
},

ToID18      :function(v){
var reg=/^[1-9]\d{5}((19|20)\d{2})(\d{2})(\d{2})\d{3}[\dxX]{1}$/;
var r=v.match(reg);
if(r==null) return null;
var d=this.ToDate(r[1]+r[3]+r[4],3);
if(d!=null)
   return v.toUpperCase();
else
   return null;
},
IsHz        :function(s){return this.IsTypeRegex(s,"hz");},
IsMac       :function(s){return this.IsTypeRegex(s,"mac");},
IsWebColor  :function(s){return this.IsTypeRegex(s,"webcolor");},
IsMobile    :function(s){return this.IsTypeRegex(s,"mobile");},
IsIP        :function(s){return this.IsTypeRegex(s,"ip");},
IsUrl       :function(s){return this.IsTypeRegex(s,"url");},
IsTypeRegex  :function(s,t) 
    { try
       {  
          t=$.trim(t).toLowerCase().split(this.typeSplit);
          
          for(var i=0;i<t.length;i++)
          {  
             var type=$.trim(t[i]);
             var r=$t.roles[type].regex;
             b=r.test(s);
             if(b==false) return false;
          }
          return true;
        }
     catch(e)
       { return null;}
    },



Add:function(type,options){   this.roles[type]=options;},

Return : function(b,o,v,type,errMsg) 
{  
   return {ok:b,value:o,result:v,type:type,msg:errMsg};

},

NInt : function (v,n)
{
    var s=$.trim(v)+"";
    var i=s.length;
    for(;i<n;i++)
      s="0" +s;
    return s;
},

Set:function(obj,name,rel,o)
{
   if( typeof(rel)!="string" ) rel=null;
   var pobj= (rel==null? $(obj).parents(".input-group").find("input"): $("#"+rel) );
   if(typeof(name) !="string") name=$.trim($(obj).text());
   r=$.fn.CheckForm.functions[name];
   if(typeof(r)=="function")
   {
       r(pobj,o);
   }
   return false;
},

Get:function(obj,name,rel)
{
   if( typeof(rel)!="string" ) rel=null;
   var pobj= (rel==null? $(obj).parents(".input-group").find("input"): $("#"+rel) );
   //if(typeof(name) !="string") name=$.trim($(obj).text());
   r=$.fn.CheckForm.functions["get"];
   if(typeof(r)=="function")
   {
       r(pobj,name);
   }
   return false;
},

SetDateRange:function(obj,value,rel)
{
   
   if( typeof(rel)!="string" ) rel=null;
   if( typeof(value)=="undefined" ) value=null;
   
   var pobj= (rel==null? $(obj).parents(".input-group").find("input.daterange,input.datetimerange"): $("#"+rel) );
   var $dr=pobj.data("daterangepicker");
   if(typeof($dr)!="object") return;
   
   if(value==null)
      value=$.trim($(obj).text());
   
   if(value!='')
   {  
       r=$.fn.daterangepicker.defaultRanges[value];
       
       if(typeof(r) == "object" )
       {
           $dr.setStartDate(r[0]);
           $dr.setEndDate(r[1]);    
           
           $dr.cb(r[0],r[1],pobj);
       }
   }
   else
   {
           //$dr.setStartDate(null);
           //$dr.setEndDate(null); 
           $dr.cb(null,null,pobj);
   }
   return false;
},

NChar:function(value,len,fillchar)
{
    var s=$.trim(value)+"";
    fillchar = this.IsNull(fillchar)?' ': $.trim(fillchar).charAt(0);
    if(len>0)
       for(var i=s.length;i< len;i++)
          s=s+fillchar;
    else
       for(var i=s.length;i<Math.abs(len);i++)
          s=fillchar + s;
    return s;
},
ToDateTime3: function(str){   return this.ToDateTime(str,3);},
ToDateTime2: function(str){   return this.ToDateTime(str,2);},
ToDateTime1: function(str){   return this.ToDateTime(str,1);},
ToDateTime4: function(str){   return this.ToDateTime(str,4);},
ToDateTime: function(str,format)
{
  //if(typeof(format)) format=1;
  var  reg  =
  format==3 ? /^(\d{4})(\d{2})(\d{2})((\d{2})((\d{2})((\d{2}))?)?)?$/ :  
  format==2 ? /^(\d{4})\/(\d{1,2})\/(\d{1,2})\s*((\d{1,2})(:(\d{1,2})(:(\d{1,2}))?)?)?$/ :  
  format==4 ? /^(\d{4})年(\d{1,2})月(\d{1,2})日((\d{1,2})时((\d{1,2})分((\d{1,2})秒)?)?)?$/ :  
  /^(\d{4})-(\d{1,2})-(\d{1,2})\s*((\d{1,2})(:(\d{1,2})(:(\d{1,2}))?)?)?$/; 
  var  r  =  $.trim(str).match(reg); 
  if(r==null)return  null; 
  r[2]=r[2]-1; 
  if(typeof(r[5])=="undefined") r[5]=0;
  if(typeof(r[7])=="undefined") r[7]=0;
  if(typeof(r[9])=="undefined") r[9]=0;
  var  d=  new Date(r[1],  r[2],r[3],  r[5],r[7],  r[9]); 
  if(d.getFullYear()!=r[1])return  null; 
  if(d.getMonth()!=r[2])return  null; 
  if(d.getDate()!=r[3])return  null; 
  if(d.getHours()!=r[5])return  null; 
  if(d.getMinutes()!=r[7])return  null; 
  if(d.getSeconds()!=r[9])return  null; 
  switch(format)
  {
   case 2:
       return  this.NInt(d.getFullYear(),4)
      +"/"+this.NInt(d.getMonth()+1,2)
      +"/"+this.NInt(d.getDate(),2)
      +" "+this.NInt(d.getHours(),2)
      +":"+this.NInt(d.getMinutes(),2)
      +":"+this.NInt(d.getSeconds(),2);
  case 3:
      return  this.NInt(d.getFullYear(),4)
      +""+this.NInt(d.getMonth()+1,2)
      +""+this.NInt(d.getDate(),2)
      +""+this.NInt(d.getHours(),2)
      +""+this.NInt(d.getMinutes(),2)
      +""+this.NInt(d.getSeconds(),2);
    case 4:
      return  this.NInt(d.getFullYear(),4)
      +"年"+this.NInt(d.getMonth()+1,2)
      +"月"+this.NInt(d.getDate(),2)
      +"日"+this.NInt(d.getHours(),2)
      +"时"+this.NInt(d.getMinutes(),2)
      +"分"+this.NInt(d.getSeconds(),2)+'秒';
  default:
      return  this.NInt(d.getFullYear(),4)
      +"-"+this.NInt(d.getMonth()+1,2)
      +"-"+this.NInt(d.getDate(),2)
      +" "+this.NInt(d.getHours(),2)
      +":"+this.NInt(d.getMinutes(),2)
      +":"+this.NInt(d.getSeconds(),2);
  }
} ,
ToDate4: function (str){  return this.ToDate(str,4);},  
ToDate3: function (str){  return this.ToDate(str,3);},  
ToDate1: function (str){  return this.ToDate(str,1);},  
ToDate2: function (str){  return this.ToDate(str,2);},  
ToDate: function (str,format)
{
  var  reg  =( format==3 ? /^(\d{4})(\d{2})(\d{2})$/ : 
  format==2 ? /^(\d{4})\/(\d{1,2})\/(\d{1,2})$/ : 
   /^(\d{4})-(\d{1,2})-(\d{1,2})$/); 
  var  r  =  $.trim(str).match(reg); 
  if(r==null)return  null; 
  r[2]=r[2]-1; 
  var  d=  new  Date(r[1],  r[2],r[3]); 
  if(d.getFullYear()!=r[1])return  null; 
  if(d.getMonth()!=r[2])return  null; 
  if(d.getDate()!=r[3])return  null; 
  switch(format)
  { case 2:
    return  this.NInt(d.getFullYear(),4)
      +"/"+this.NInt(d.getMonth()+1,2)
      +"/"+this.NInt(d.getDate(),2);
    case 3:
      return  this.NInt(d.getFullYear(),4)
      +""+this.NInt(d.getMonth()+1,2)
      +""+this.NInt(d.getDate(),2);
     case 4:
      return  this.NInt(d.getFullYear(),4)
      +"年"+this.NInt(d.getMonth()+1,2)
      +"月"+this.NInt(d.getDate(),2)+'日';
    default:
      return  this.NInt(d.getFullYear(),4)
      +"-"+this.NInt(d.getMonth()+1,2)
      +"-"+this.NInt(d.getDate(),2);
   }
},
ToFloat:function(str)
{
   var n=parseFloat(str);
   if(isNaN(n)) 
      return null;
   else
      return n;
},

ToInt:function(str)
{
   var n=parseInt(str,10);
   if(isNaN(n)) 
      return null;
   else
      return n;
},

ToNumberX: function(str,n)
{
   str=$.trim(str);
   if(str=="" || str==null) return null;
   if(typeof(n)=="undefined") n=null;
   if(n==null ||n<0) n=0;
   str=_(str).toNumber(n);
   if(isNaN(str)) 
       return null;
   else
       return str;
},

ToNumber: function(str)
{
     str=$.trim(str);
     if(this.IsNumber(str)==null) return null;
     try{
       var s=Number(str);
       if(isNaN(s)) return null;
          else      return s;
       }
       catch(e) { return null;}
},
ToBit:function(str)
{
    if(this.IsNull(str)) return null;
    return $.trim(str)?1:0;
},
ToBool:function(str)
{
    if(this.IsNull(str)) return null;
    return ($.trim(str)+"").toLowerCase()=="true" ?true:false;
},

SubAdd: function(obj,IsAdd)
{
  return this.AddSub(obj,IsAdd);
},

AddSub: function(obj,IsAdd)
{
     var rel=$(obj).parents(".input-group").find("input");
     if(rel.length==1)
       {
           var step = rel.attr("step");
           step = this.ToNumber(step);
           if(step==null) step=1;
           var min =this.ToNumber(rel.attr("min"));
           var max =this.ToNumber(rel.attr("max"));
           var val =this.ToNumber(rel.val());
           if(val==null) val=0;
           val+=(IsAdd? step:-step);
           if(min!=null && val<min) val=min;           
           if(max!=null && val>max) val=max;
           rel.val(val);
      }
    
    return false;
},

Val:function(str,val)
{
  if (typeof (val) == "undefined") val = null;
  if(typeof(str)=="undefined" || str==null || $.trim(str)=="")  return val;
  else  return $.trim(str);
},


ToTime1: function(str,format){return this.ToTime(str,1);},
ToTime2: function(str,format){return this.ToTime(str,2);},
ToTime3: function(str,format){return this.ToTime(str,3);},
ToTime4: function(str,format){return this.ToTime(str,4);},
ToTime: function(str,format)
{ format=this.Val(format,1);
  var  reg  =(format==3?  /^(\d{2})(\d{2})(\d{2})?$/  
  :  /^(\d{1,2}):(\d{1,2})(:(\d{1,2}))?$/); 
  var  r  =  $.trim(str).match(reg); 
  if(r==null)return  null; 
  r[2]=r[2]-1; 
  if(typeof(r[4])=="undefined") r[3]=0;
  var  d=  new  Date(1970,1,1,r[1],  r[2],r[3]); 
  if(d.getHours()!=r[1])return  null; 
  if(d.getMinutes()!=r[2])return  null; 
  if(d.getSeconds()!=r[3])return  null;
  switch(format)
  {
  case 3:
  return this.NInt(d.getHours(),2)
  +""+this.NInt(d.getMinutes(),2)
  +""+this.NInt(d.getSeconds(),2);
  case 4:
  this.NInt(d.getHours(),2)
  +"时"+this.NInt(d.getMinutes(),2)
  +"分"+this.NInt(d.getSeconds(),2)+"秒";
  default:
  return this.NInt(d.getHours(),2)
  +":"+this.NInt(d.getMinutes(),2)
  +":"+this.NInt(d.getSeconds(),2);
  }
},

IsRegex   :function(reg,str){return reg.test($.trim(str));},

Len:function(s,unicode)
{ s=(s? s:"");
  var len=s.length;
  if(unicode === false) 
  {   var n=0;
      var reg=/[^\x00-\xff]/;
      for(i=0;i<len;i++)
      { if (reg.test(s.substring(i,i+1))) n++;}
      len +=n;
  }
  return len;
},

InnerType:function(type)
{
 if(this.IsNull(type)) type="varchar";
 type=type.toLowerCase();
 switch(type)  //max目前只支持2种
  {  case "varchar(max)": type="text";break;
     case "nvarchar(max)": type="ntext";break;
     case "string":
     case "char":   type="varchar";break;
     case "nstring":
     case "nchar": type="nvarchar";break;
     case "number":type="decimal";break;
     case "tinyint":type="byte";break;
     case "bigint":type="long";break;
     case "boolean":type="bool";break;
     case "interger":type="int";break;
     case "smallint":type="short";break;
  }
  return type.replace("(max)","");
},


JsonText:function(d)
{
    return ("{\nok:"+d.ok
    +",\nvalue:'"+d.value+"',\nresult:'"+(d.result==null?"null":d.result)
    +"',\ntype:'"+d.type+"',\nmsg:'"+d.msg+"'\n}");
},

Show:function(d)
{
    alert(this.JsonText(d));
},

ResultN:function(value,types,options)
{
   var s=this.CheckX(value,null,types,options);
   return s.result;
},

ResultX:function(values,split,types,options)
{
   var s=this.CheckX(values,split,types,options);
   return s.result;
},

Result:function(value,type,options)
{
   var s=this.Check(value,types,options);
   return s.result;
},



IsX:function(valus,split,types,options)
{
   var s=this.CheckX(valus,split,types,options);
   return s.ok;
},

IsN:function(value,types,options)
{
   var s=this.CheckN(value,types,options);
   return s.ok;
},

ShowX:function(options)
{
 var s="";
   for(var i in options)
   {
       s +="\t"+i + "="+options[i]+"\n";
   }
   alert("{\n" + s +"\n}");
 },  
Is:function(value,type,options)
{
   var s=this.Check(value,types,options);
   return s.ok;
},


CheckN:function(values,types,options)
{
  return this.CheckX(values,null,types,options);
},

CheckX:function(values,split,types,options)
{ 
   var valueList=null;
   var row=1;
   var b=true;
   var r=this.Return(false,values,"",types,"未知结果");
   if(split==null || split=="")
     { valueList=[values];
       b=false;
     }
   else
   {
     valueList=values.split(split);
     row=valueList.length;
     b=true;
     if( options &&  options.mincount && row < options.mincount)
        return this.Return(false,values,null,types,"多项值中所需要的个数必须>=" + options.mincount+",目前个数="+row);
     if(options && options.maxcount && row > options.maxcount)
        return this.Return(false,values,null,types,"多项值中所需要的个数必须<=" + options.maxcount+",目前个数="+row);
    }
   var toValues="";
   for(var i=0;i<row;i++)
   {
      var value=$.trim(valueList[i]);
      var typelist=types.split(this.typeSplit);
      for(var ti=0;ti<typelist.length;ti++)
        {
          var type=$.trim(typelist[ti]);
          if(type!="")
           {
              r=this.Check(value,type,options);
              //this.Show(r);
              if(!r.ok)
              {
               r.value=values;
               if(b && row>1)
                 r.msg="转换多项值“"+values+"”中的“"+value+"”为类型“"+type+"”时出错,"+r.msg;
               return r;
              }
              
           }
       }
      toValues += r.result +(i<row-1? split:"")
   }
   
  var vlen=this.Len(values,false) ;//d.unicode); //取得当前值的长度
  if(options && options.zminlength  && options.zminlength>0)
  {
    b=(vlen>=options.zminlength);
    if(!b)
        return this.Return(false,values,null,type,"总长度必须>=" +options.zminlength);
  }
  
  if(options &&  options.zmaxlength  && options.zmaxlength>0)
  {
     b=(vlen<=options.zmaxlength);
     if(!b)
        return this.Return(false,values,null,type,"总长度必须<=" +options.zmaxlength); 
   }
  
   return this.Return(true,values,toValues,types,"检查成功");
},
CheckForm:function(form)
{
    var b = true;
    $(form).find("input[vType]:enabled,select[vType]:enabled,textarea[vType]:enabled").each(
        function () {
            b = b && $t.CheckObj($(this));
        }
        );
    return b;
},
 
CheckObj:function(o)
{
    var type = this.Val($(o).attr("vtype"), null);
    var isnull=this.Val($(o).attr("isnull"));
    if (isnull == null)
        isnull= $(o).attr("required")?false:true;
    else
        isnull = (isnull=="1" || isnull=="true");
    var opt = 

    {
        min: this.Val($(o).attr("min")),
        max: this.Val($(o).attr("max")),
        minlength: this.Val($(o).attr("minlength")),
        maxlength: this.Val($(o).attr("maxlength")),
        mincount: this.Val($(o).attr("mincount")),
        maxcount: this.Val($(o).attr("maxcount")),
        check: this.Val($(o).attr("check")),
        rang: this.Val($(o).attr("rang")),
        zminlength: this.Val($(o).attr("zminlength")),
        zmaxlength: this.Val($(o).attr("zmaxlength")),
        isnull:  isnull,
        title: this.Val($(o).attr("title"), $(o).attr("name")),
        msg: this.Val($(o).attr("msg"), null)
    };
    var s = this.Val($(o).attr("regex"));
    if (s != null)
        opt.regex = s;
    var r = $t.Check($(o).val(), type, opt);
    if (!r.ok) {
        alert(opt.msg == null ? (opt.title == null ? "" : opt.title )+r.msg : opt.msg);
        $(o).focus();
        return false;
    }
    else
        return true;
},
FormErrResult:function()
{
      alert("无法保存当前的表单内容");
      return false;
},
FormID:function()
{
    return "form#frmEdit";
},
FormData:function()
{
    var b = false;
    var formID = $t.FormID();
    var fnCheck = $(formID).attr("fnCheck");
    if (typeof (fnCheck) == "string")
    {
        try
        {
            b = eval(fnCheck + "()");
            if (!b) return null;
        }
        catch(e)
        {
            alert("获取检测函数信息出错，请重新检查");
            return null;
        }
    }
    b = $t.CheckForm($(formID));
    if (b == false)  
        return null;
    else 
        return $(formID).serialize();
},
FormSaveResult: function (result)
{
    var formID = $t.FormID();
    if (result.ok == 0) {
        alert("更新出错了，错误信息 = " + result.msg);
    }
    else
    {
        $(formID).attr("IsReloadWhenClose", "1");
        var actionUpdate = $(formID).attr("actionUpdate");
        actionUpdate =$(formID).attr("action") + "?action=" +(actionUpdate ? actionUpdate : "update");
        $(formID+" #bntSave,"+formID+" #bntSaveEnd").attr("href", actionUpdate);
        alert("记录保存成功，请继续编辑！");
    }
},
FormInit:function()
{
    var formID = $t.FormID();
    $(formID+" #bntSave,"+formID+" #bntSaveEnd,"+formID+" #bntDelete")
    .attr("method", "post")
    .attr("fnData", "$t.FormData")
    .attr("dataType", "json")
    .attr("fnErr", "$t.FormErrResult")
    .click(function () { return OA.i(this) })
    .each(function () {
        $(this).attr("href", $(formID).attr("action") + "?action=" + $(this).attr("action"))
    });
    $(formID +" #bntSave").attr("fnCall", "$t.FormSaveResult");
    $(formID + " #bntSaveEnd").attr("fnCall", "$t.FormSaveEndResult");
    $(formID + " #bntDelete").attr("fnCall", "$t.FormDeleteResult");
    $(formID + " #bntClose").click(function () { $t.FormClose(); });
},
FormDeleteResult: function (result) {
    if (result.ok == 0) {
        alert("删除出错了，错误信息 = " + result.msg);
    }
    else
    {
        OA.CloseMe(true);
    }
},
FormSaveEndResult: function (result) {
    if (result.ok == 0) {
        alert("更新出错了，错误信息 = " + result.msg);
    }
    else
    {
        OA.CloseMe(true);
    }
},
FormClose:function()
{
    var formID = $t.FormID();
    OA.CloseMe($(formID).attr("IsReloadWhenClose")=="1");
},
Check: function(value,type,options)
{
  
  var d={};
  var v,len=null,scale=null;
  var min=null,max=null;
  var minlength=0,maxlength=null;
  var b=false;
  var tv=null;
  
  v =$.trim(value);
  if(v=="") v=null;
  
  type=this.InnerType(type);

  var s=type.split('(');
  if(s.length>1)
    {
      type=s[0];
      s=s[1].replace(')','').split(',');
      if(s.length>1) { len=s[0]; scale=s[1];}
      else           { len=s[0]; scale=null;}
      
      len=this.ToInt(len);
      if(len!=null && len<1)
      {
         return  this.Return(false,v,null,type,"要求的字节个数或者精度不能<1");
      }
      
      scale=this.ToInt(scale); //是否为要求的精度
      
      if(len==null && scale!=null)
      {
         return  this.Return(false,v,null,type,"小数位数定义了但精度没定义");
      }
      
      if(len!=null && scale!=null &&( scale<0 || scale>len-1))
      {
         return  this.Return(false,v,null,type,"小数位数不能小于0或者大于精度-1");
      }
    }
    else
    { 
      len=null;
      scale=null;
    }
    
    type=this.InnerType(type); //重新取得innerType值
    
    try
    {
        d=this.roles[type];
        if(len!=null) d.maxlength=len;
        if(scale!=null) d.scale = scale;
        if(typeof(d.isnull)=="undefined") d.isnull=false;
      }
    catch(e)
    {
        return this.Return(false,v,null,type,"无法找到相关的类型["+type+"]");
    }
  
    if ($t.Val(d.min) == null && d.bit)
        d.min = -Math.pow(2,d.bit) + 1;
    if($t.Val(d.max) ==null && d.bit)
  　    d.max = Math.pow(2,d.bit) - 1;
   
  min=$t.Val(d.min);
  max=$t.Val(d.max);
   
  
  d=$.extend({}, d, options); 
  if(!d.rang) d.rang="[]";
  
  if(typeof(d.isnull)=="undefined") d.isnull=true;
  
  
  //this.ShowX(d);
  
  //alert("d.isnull="+ d.isnull +",v="+(v==null?"null":v));
  
  if(v==null)
  {
     if(this.ToBool(d.isnull) == true)       
         return this.Return(true,v,"",type,"检查成功");
    else
         return this.Return(false,v,null,type,"不能为空值");
   
   } 
  scale =$t.Val(d.scale);// )=="undefined"? null:d.scale;
  //OA.ShowJson(d);
  if(d.regex)
  {
      var r=new RegExp(d.regex);
      b= r.test(v);
      if(!b)
        return  this.Return(false,v,null,type,"[" + v + "]不是要求的“"+d.name+"”");
  }
  
  
   if( min!=null  && typeof( d.min )!="undefined" && min < d.min) min=d.min;
   if( max!=null  && typeof( d.min )!="undefined" && max > d.max ) max=d.max;


switch(d.type)
{ 
case "b":
      tv=this.ToBit(v);
      if(tv==null)
        return this.Return(false,v,null,type, "无法转换成bit型");
      else    
        return this.Return(true,v,tv,type, "检查成功");
case "bool":
      tv=this.ToBool(v);
      if(tv==null)
         return this.Return(false,v,null,type, "无法转换成bool型");
      else
         return this.Return(true,v,tv,type, "检查成功");
case "n": 
      //if(len!=null)  //处理精度
      //{   if(len>18) 
      //     {
      //      return this.Return(false,v,null,type, "数值的精度不能超过38位或者小于1位");
      //     }
      //    //if(scale==null) 
      //    len =len-scale;
      //   if(min==null)  min = - Math.pow(10.0,len)+1;
      //   if (max == null) max = Math.pow(10.0, len) - 1;
          
      //}

      //alert(len);

      //if (scale != null) {
      //    tv = this.ToNumberX(v, scale);
      //   // min = this.ToNumberX(min, scale);
      //   // max = this.ToNumberX(max, scale);
      //}
      //else
      //{
    try {
        v = OA.rmoney(v);
        tv = Number(v);
        if (isNaN(tv)) tv = null;
    }
    catch (e)
    { tv = null; }
        //  min = Number(min);
         // max=Number(max)
      //}
      
      if(tv==null)  return this.Return(false,v,null,type, "["+v+"]不是要求的数值型数");
       
      break;
case "i":
    tv = this.ToInt(v);
    
    if (tv == null) return this.Return(false, v, null, type, "[" + v + "]不是要求的整型数");
    min = this.ToInt(min);
    max = this.ToInt(max);

      break;
case "f":
      tv=this.ToFloat(v);
      if (tv == null) return this.Return(false, v, null, type, "[" + v + "]不是要求的浮点数");
      min = this.ToFloat(min);
      max = this.ToFloat(max);
      break;
case "dt":
         tv =this.ToDateTime(v,d.df);
         if(tv==null)
         { 
           var stv=this.ToDate(v,d.df);
           if(stv==null)
               return this.Return(false,v,null,type,"["+v+"]不是日期时间格式["+d.format+"]");
           else
               tv= stv + d.time;
         }
         min = this.ToDateTime(min, d.df);
         max = this.ToDateTime(max, d.df);
         break;
case "d":
     tv =this.ToDate(v,d.df);
     if(tv==null) return  this.Return(false,v,null,type,"["+v+"]不是日期格式["+d.format+"]");
     min = this.ToDate(min, d.df);
     max = this.ToDate(max, d.df);
     break;
case "t":
     tv = this.ToTime(v, d.df);
      if(tv==null)   return  this.Return(false,v,null,type,"["+v+"]不是时间格式["+d.format +"]");
      min = this.ToTime(min, d.df);
      max = this.ToTime(max, d.df);
      break;
 default:
      d.type="s"; 
      tv=v;
      break;  
  }

  d.scale=scale;
  //if(min==null) min=d.min;
  //if(max==null) max=d.max;
  
  var vlen=this.Len(v,d.unicode); //取得当前值的长度
  if(d.minlength && d.type=="s" )
  {
    b=(vlen>=d.minlength);
    if(!b)
        return this.Return(false,v,null,type,"长度必须>=" +d.minlength);
  }
  
if(d.maxlength && d.type=="s")
  {
     b=(vlen<=d.maxlength);
     if(!b)
        return this.Return(false,v,null,type,"长度必须<=" +d.maxlength); 
  }
  
if(typeof(min)!="undefined" && min!=null)
{   var ts=(d.rang.charAt(0)=='[');
    b=( ts? (tv>=min) : (tv>min));
    if(!b)
        return  this.Return(false,v,null,type,"值["+ tv +"]需要"+(ts?">=":">") +min);
}
  
if(typeof(max)!="undefined" && max!=null)
{  var ts=(d.rang.charAt(1)==']');
   b=(ts ? (tv<=max) : (tv<max));
   if(!b)
      return  this.Return(false,v,null,type,"值["+ tv +"]需要"+(ts? "<=":"<") +max);
}

if(d.check) //定義的自定義
 {
   b=d.check(tv);
   if(!b) return this.Return(false,v,null,type,(d.msg?d.msg:'自定义检测失败'));
 }


return  this.Return(true,v,tv,type,"检查成功");

}
};

