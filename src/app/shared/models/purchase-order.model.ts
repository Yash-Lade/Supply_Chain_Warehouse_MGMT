export interface Approval {
  level: number;
  role: string;
  status: string;
}

export interface PurchaseOrder {
  id: number;
  poNumber: string;
  vendorName: string;
  warehouseId: number;
  status: string;
  totalAmount: number;
  createdAt: string;
  approvals: Approval[];
}