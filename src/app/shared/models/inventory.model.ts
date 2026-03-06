export interface InventoryItem {
  itemId: number;
  itemName: string;
  warehouseName: string;
  currentStock: number;
  rol: number;
  criticalLevel: number;
}