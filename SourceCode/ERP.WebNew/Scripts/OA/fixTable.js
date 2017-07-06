// JScript 文件


 

            
function FixTable(options) 
{

       var opts = $.extend(
            {},
            { width:'100%',
              height:'100%',
              bgcolor:'Silver',
              whiteSpace:"nowrap", 
              showEmptyCells: true,
              /*
              normal 默认。空白会被浏览器忽略。 
pre 空白会被浏览器保留。其行为方式类似 HTML 中的 <pre> 标签。 
nowrap 文本不会换行，文本会在在同一行上继续，直到遇到 <br> 标签为止。 
pre-wrap 保留空白符序列，但是正常地进行换行。 
pre-line 合并空白符序列，但是保留换行符。 
inherit 规定应该从父元素继承 white-space 属性的值。 */
              fixedCols:1,
              textAlign:'center', //center ,left right
              id:null,
              tableLayout:'fixed',
              cellEllipsis:true        
            } ,
            options);   
  
            var id= opts.id;
            var _id="#" + opts.id;
            
            var pid= id +"_tableLayout";
            var bAutoSize=false;
            var _pid="#"+ pid;
            
            var oldtable = $(_id).css({
             "empty-cells":(opts.showEmptyCells?"show":"hide"),
            "table-layout":opts.tableLayout} );
            
            if(oldtable.length==0) return;
            
            
            var w =oldtable.width();
            
            var width=opts.width;
            var height=opts.height;
            var cw=window.innerWidth|| document.documentElement.clientWidth||document.body.clientWidth;
            var ch=window.innerHeight || document.documentElement.clientHeight|| document.body.clientHeight;
            var color =opts.bgcolor;
            var bAutoSize =false;
            
            if(opts.width=="100%") { width= cw; bAutoSize=true;}
            if(opts.height=="100%") { height= ch; bAutoSize=true;}
            
            var fixedCols =opts.fixedCols;
            
            if(width > cw) width= cw;
            if(height >ch) height= ch;
            
            if(bAutoSize) //需要自动改变大小
            {
                $(window).resize( function()
                {
                    var width = opts.width;
                    var height = opts.height;
                    if(opts.width=="100%") { width =(window.innerWidth|| document.documentElement.clientWidth||document.body.clientWidth); }
                    if(opts.height=="100%"){ height=(window.innerHeight || document.documentElement.clientHeight|| document.body.clientHeight);}
                    
                    $(_pid).css({ width: width,height:height});
                     $(_id + "_tableHead").css({"width": width - 17});
			        $(_id + "_tableColumn").css({"height": height - 17});
			        $(_id + "_tableData").css({"width": width, "height": height});
                
                });
            }
            if( w<width) width=w;
            
            
            
			if ($(_pid).length != 0) 
			{
				$(_pid).before($(_id));
				$(_pid).empty();
			}
			else 
			{
				$(_id).after("<div id='" + pid + "' style='overflow:hidden;height:" + height + "px; width:" + width + "px;'></div>");
			}
			
			/*var css ="<style>"+_pid +" table {"
            +"empty-cells:" +(opts.showEmptyCells?"show":"hide")+","
            +"table-layout:" + opts.tableLayout +"}";
            */
            var  css ="<style>"+_pid + ' table td,'+_pid+' table th{overflow:hidden;'
             +'text-overflow:'+ ( opts.cellEllipsis ? 'ellipsis':'clip') +';'
             +'white-space:'+( opts.whiteSpace ? opts.whiteSpace :'nowrap')+';'
             +'text-align:' + ( opts.textAlign ? opts.textAlign :'center')
             +'}</style>'
            $(css+  '<div style="overflow:hidden;position:absolute;left:0px;top:0px;z-index:50;background-color:'+color+'" id="' + id + '_tableFix"><div style="width:'+w+'px"></div></div>'
			+ '<div style="overflow:hidden;position:absolute;left:0px;top:0px;z-index:45;background-color:'+color+'" id="' + id + '_tableHead"><div style="width:'+w+'px"></div></div>'
			+ '<div style="overflow:hidden;position:absolute;left:0px;top:0px;z-index:40;background-color:'+color+'" id="' + id + '_tableColumn"><div style="width:'+w+'px"></div></div>'
			+ '<div style="overflow:scroll;position:absolute;left:0px;top:0px;z-index:35;" id="' + id + '_tableData"><div style="width:'+w+'px"></div></div>')
			.appendTo(_pid);
			
			var t = oldtable.clone(true).attr("id", id + "_tableFixClone");
			$(_id + "_tableFix").find("div").append(t);
       
       
            t = oldtable.clone(true).attr("id", id + "_tableHeadClone");
			$(_id + "_tableHead").find("div").append(t);
       
       
            t = oldtable.clone(true).attr("id", id + "_tableColumnClone");
			$(_id + "_tableColumn").find("div").append(t);
			
            $(_id + "_tableData").find("div").append(oldtable);
            
			$(_pid+" table").each(function () {
				$(this).css("margin", "0");
			});

			var HeadHeight = $(_id + "_tableHead thead").height();
			HeadHeight += 2;
			$(_id + "_tableHead").css("height", HeadHeight);
			$(_id + "_tableFix").css("height", HeadHeight);


			var ColumnsWidth = 0;
			var ColumnsNumber = 0;
			$(_id + "_tableColumn tr:last td:lt(" + fixedCols + ")").each(function () {
				ColumnsWidth += $(this).outerWidth(true);
				ColumnsNumber++;
			});
 		   ColumnsWidth += 2;
  
			$(_id + "_tableColumn").css("width", ColumnsWidth);
			$(_id + "_tableFix").css("width", ColumnsWidth);
	        $(_id + "_tableHead").css({"width": width - 17});
			$(_id + "_tableColumn").css({"height": height - 17});
			$(_id + "_tableData").css({"width": width, "height": height});
			

			$(_id + "_tableData").scroll(function ()
			 {
				$(_id + "_tableHead").scrollLeft($(_id + "_tableData").scrollLeft());
				$(_id + "_tableColumn").scrollTop($(_id + "_tableData").scrollTop());
			});

		  $(window).resize();

}
