/*
 * 基于HTML5 文件上传的核心脚本
 * 暂支持单个文件上传
 * by Thyiad 2015-08-23
*/
var h5uploader = {
	fileInput: null,				//html file控件
	upButton: null,					//提交按钮
	url: "",						//ajax地址
	file:'',
	filter:function(file){return file;},		//文件选择过滤
	onSelect: function() {},		//文件选择后
	onProgress: function() {},		//文件上传进度
	onSuccess: function() {},		//文件上传成功时
	onFailure: function() {},		//文件上传失败时,
	
	//获取文件
	funGetFiles:function(e){
		var files = e.target.files || e.dataTransfer.files;
		var f=files[0];
		if(f){
			f=this.filter(f);
			if(f){
				this.onSelect(f);
			}
			this.file = f;
		}
	},
	funUploadFile:function(e){
		var self =this;
		if(!self.file){
			alert('请选择文件！');
			return;
		};
		//if (!/^[A-z\-_\.\w]+$/.test(self.file.name)) {
		//    alert('暂只支持英文名文件上传！');
		//    return;
		//}
		self.upButton.disabled = true;
		(function (file) {
			var xhr=new XMLHttpRequest();
			if(xhr.upload){
				// 上传中
				xhr.upload.addEventListener('progress',function (e) {
					self.onProgress(file,e.loaded,e.total);
				},false);
				
				// 上传成功或失败
				xhr.onreadystatechange=function (e) {
					if (xhr.readyState === 4) {
						if (xhr.status === 200) {
							self.onSuccess(file,xhr.responseText);
						} else{
							self.onFailure(file,xhr.responseText);
						}
						self.upButton.disabled = false;
					}
				};
				var formData = new FormData();
				formData.append('file', file);
				// 开始上传
				xhr.open('POST',self.url,true);
				xhr.setRequestHeader('X_FILENAME',escape(file.name));
				xhr.send(formData);
			}
			else{
				alert('浏览器版本过低，XMLHttpRequest不支持upload!')
			}
		})(self.file);
	},
	
	init: function() {
		var self = this;
		
		//文件选择控件选择
		if (this.fileInput) {
			this.fileInput.addEventListener("change", function(e) { self.funGetFiles(e); }, false);	
		}
		
		//上传按钮提交
		if (this.upButton) {
			this.upButton.addEventListener("click", function(e) { self.funUploadFile(e); }, false);	
		} 
	}
};