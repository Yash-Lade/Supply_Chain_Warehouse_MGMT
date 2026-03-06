import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../../core/services/auth.service';
import { PurchaseOrderService } from '../../../core/services/purchase-order.service';
import { MatCard } from "@angular/material/card";
import { MatTableModule } from '@angular/material/table';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
@Component({
selector:'app-pending-approvals',
standalone:true,
templateUrl:'./pending-approvals.component.html',
imports: [
    MatCard, 
    MatTableModule,
    MatButtonModule,
    CommonModule
]
})
export class PendingApprovalsComponent implements OnInit {

columns = ['poNumber','vendor','status','action'];

dataSource:any[]=[];

role!:number;

constructor(
private auth:AuthService,
private poService:PurchaseOrderService
){}

ngOnInit(){

this.role = this.auth.getRole();

this.loadPOs();

}

loadPOs(){

this.poService.getAll().subscribe(res=>{
this.dataSource = res.filter((p:any)=>p.status === 'PendingApproval');
});

}

canApprove(po:any){

if(this.role === 2 && po.approvalLevel === 1) return true;

if(this.role === 3 && po.approvalLevel === 2) return true;

if(this.role === 4 && po.approvalLevel === 3) return true;

return false;

}

approve(id:number){

this.poService.approve(id,{
action:'Approved'
}).subscribe(()=>{

this.loadPOs();

});

}

reject(id:number){

this.poService.approve(id,{
action:'Rejected'
}).subscribe(()=>{

this.loadPOs();

});

}

}

